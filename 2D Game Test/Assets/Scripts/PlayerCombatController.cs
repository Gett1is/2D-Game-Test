using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField]
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage, maxAttack1Damage, attackCharger;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask isDamageable;

    private bool gotInput, isAttacking, isChargingAttack, isFirstAttack, canChargeAttack;

    private float lastInputTime;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        inputTimer = Mathf.NegativeInfinity;
        isChargingAttack = false;
        attackCharger = 0.0f;
        attack1Damage = 2.0f;
        maxAttack1Damage = 10.0f;
    }
    private void Update()
    {
        CheckCombatInput();
        CheckCombatEnabled();
        CheckAttacks();
        CheckChargeAttack();
        UpdateAnimations();
        Debug.Log(PlayerController.isInFightStance);
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isChargingAttack", isChargingAttack);
        anim.SetFloat("attackCharge", attackCharger);
    }

    private void CheckCombatInput()
    {
        if(Input.GetMouseButton(0))
        {
            if(combatEnabled)
            {
                ChargeAttack();
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            ResetValuesOnExit();
        }
    }

    private void CheckAttacks()
    {
        if(gotInput)
        {
            if (!isAttacking)
            {
                isChargingAttack = false;
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack1", true);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }


    }
    private void ResetValuesOnExit()
    {
        isChargingAttack = false;
        attackCharger = 0.0f;
        FinishAttack1();
    }
    private void CheckChargeAttack()
    {
        if (combatEnabled)
        {
            canChargeAttack = true;
        }
        else
        {
            canChargeAttack = false;
        }
    }

    private void ChargeAttack()
    {
        if (canChargeAttack)
        {
            isChargingAttack = true;
            if (attack1Damage + attackCharger < maxAttack1Damage)
            {
                attackCharger += Time.deltaTime * 4f;
            }
        }
        else
        {
            isChargingAttack = false;
        }
    }
    private void CheckCombatEnabled()
    {
        if(PlayerController.isInFightStance)
        {
            combatEnabled = true;
        }
        else
        {
            combatEnabled = false;
        }
    }

    private void CheckAttack1HitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, isDamageable);
        foreach(Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attack1Damage);
        }
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
