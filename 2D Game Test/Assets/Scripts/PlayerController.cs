using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;

    private Animator animator;

    public Transform groundCheck;
    public Transform wallCheck;

    public Vector2 wallHopDir;
    public Vector2 wallJumpDir;

    public LayerMask whatIsGround;

    // --- Movement vars ---
    private float movementInputDir;
    private float maxJump;
    private float minMovementSpeed;
    private float jumpCharger;
    private float fightStanceForwardVelocity;
    //private float attackCharger;

    public float jumpForce;
    public float groundCheckRadius;
    public float movementSpeed;
    public float fightStanceMovementSpeed;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier;
    public float wallHopForce;
    public float wallJumpForce;
    //public float attackPower;
    //public float maxAttackPower;
    //public float playerVelocityCap;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded = false;
    private bool canJump;
    private bool canChargeJump;
    private bool isChargeJumping;
    private bool isTouchingWall;
    private bool isWallSliding;
    public static bool isInFightStance;
    private bool canEnterFightStance;
    //private bool canAttack;
    //private bool isAttacking;
    //private bool isChargingAttack;
    //private bool canChargeAttack;

    private int amountOfJumpsLeft;
    private int facingDir = 1;

    public int amountOfJumps = 1;
    

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        wallHopDir.Normalize();
        wallJumpDir.Normalize();
        movementSpeed = 2f;
        fightStanceMovementSpeed = 0.5f;
        jumpForce = 2f;
        jumpCharger = 0f;
        maxJump = 2.5f;
        //attackCharger = 0f;
        //attackPower = 2.0f;
        //maxAttackPower = 10.0f;
        minMovementSpeed = 0.1f;
        isWallSliding = false;
        isInFightStance = false;
        canEnterFightStance = false;
        isChargeJumping = false;
        //canChargeAttack = false;
        amountOfJumpsLeft = amountOfJumps;
        airDragMultiplier = 0.95f;
        movementForceInAir = 5.0f;
        //playerVelocityCap = 4.0f;
    }

    void Update()
    {

        CheckInput();
        CheckMovementDir();
        CheckIfCanJump();
        CheckIfCanChargeJump();
        CheckIfWallSliding();
        CheckFightStance();
        //CheckChargeAttack();
        //CheckAttack();
        UpdateAnimations();

    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && playerRigidbody.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void CheckIfCanChargeJump()
    {
        if(isGrounded && !isInFightStance || isWallSliding)
        {
            canChargeJump = true;
        }
        else
        {
            canChargeJump = false;
        }
    }
    private void CheckIfCanJump()
    {
        if ((isGrounded && playerRigidbody.velocity.y <= 0) || isWallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if(amountOfJumpsLeft <= 0 || isInFightStance)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("speed", Mathf.Abs(movementInputDir));
        animator.SetBool("grounded", isGrounded);
        animator.SetFloat("jumpCharge", jumpCharger);
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("yVelocity", playerRigidbody.velocity.y);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("isInFightStance", isInFightStance);
        animator.SetFloat("fightStanceVelocity", fightStanceForwardVelocity);
        //animator.SetFloat("attackCharge", attackCharger);
        //animator.SetBool("isChargingAttack", isChargingAttack);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckInput()
    {
        movementInputDir = Input.GetAxisRaw("Horizontal");

        if (Input.GetButton("Jump"))
        {
            ChargeJumpPower();
        }
        if (Input.GetButtonUp("Jump"))
        {
            Jump();
        }
        if (Input.GetButton("Fire2"))
        {
            EnterFightStance();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            ExitFightStance();
        }
        /*if (Input.GetButton("Fire1"))
        {
            ChargeAttack();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Attack();
        }*/
    }

    /*private void CheckAttack()
    {
        if(isInFightStance && isChargingAttack)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }
    private void CheckChargeAttack()
    {
        if (isInFightStance)
        {
            canChargeAttack = true;
        }
        else
        {
            canChargeAttack = false;
        }
    }

    private void Attack()
    {
        if(canAttack && isChargingAttack)
        {
            isChargingAttack = false;
            attackPower += attackCharger;
            Debug.Log("ATTACK" + attackPower + " DMG");
            //isAttacking = true; <--------------- hur???
            attackCharger = 0f;
            attackPower = 2.0f;
            // Attack kod
        }
        else
        {
            isAttacking = false;
        }
    }

    private void ChargeAttack()
    {
        if(canChargeAttack)
        {
            isChargingAttack = true;
            if (attackPower + attackCharger < maxAttackPower)
            {
                attackCharger += Time.deltaTime * 4f;
            }
        }
        else
        {
            isChargingAttack = false;
        }
    }
    */

    private void CheckFightStance()
    {
        if (isGrounded && !isChargeJumping)
        {
            canEnterFightStance = true;
        }
        else if (!isGrounded || isChargeJumping)
        {
            canEnterFightStance = false;
        }
    }

    private void EnterFightStance()
    {
        if(canEnterFightStance)
        {
            isInFightStance = true;
        }
        else if(!canEnterFightStance)
        {
            isInFightStance = false;
        }

        if(isFacingRight)
        {
            fightStanceForwardVelocity = playerRigidbody.velocity.x;
        }
        else if(!isFacingRight)
        {
            fightStanceForwardVelocity = -(playerRigidbody.velocity.x);
        }
    }

    private void ExitFightStance()
    {
        isInFightStance = false;
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void Jump()
    {
        if(canJump && !isWallSliding) // GroundJump
        {
            //playerRigidbody.AddForce(new Vector2(playerRigidbody.velocity.y, jumpForce + jumpCharger), ForceMode2D.Impulse); //Ett sätt att hoppa
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce + jumpCharger);
            movementSpeed = 2.0f;
            jumpCharger = 0.0f;
            wallSlideSpeed = 0.5f;
            amountOfJumpsLeft--;
            isChargeJumping = false;
        }
        else if(isWallSliding && movementInputDir == 0 && canJump) // WallHop
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Flip();
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDir.x * facingDir, wallHopForce * wallHopDir.y);
            playerRigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);
            movementSpeed = 2.0f;
            wallSlideSpeed = 0.5f;
            jumpCharger = 0.0f;
            isChargeJumping = false;
        }
        else if((isWallSliding || isTouchingWall) && movementInputDir != 0 && canJump) // WallJump
        {
            isWallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2((wallJumpForce + jumpCharger) * wallJumpDir.x * -facingDir, (wallJumpForce + jumpCharger) * wallJumpDir.y);
            playerRigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);
            movementSpeed = 2.0f;
            wallSlideSpeed = 0.5f;
            jumpCharger = 0.0f;
            isChargeJumping = false;
        }
    }

    private void ChargeJumpPower()
    {
        if(canChargeJump) // Cant use "canJump" since it has duoblejump properties and we only want to charge when grounded
        {
            isChargeJumping = true;

            if (isGrounded)
            {
                if (movementSpeed > minMovementSpeed)
                {
                    movementSpeed -= Time.deltaTime * 4f;
                }
                else
                {
                    movementSpeed = minMovementSpeed;
                }
            }
            else if(isWallSliding)
            {
                if(wallSlideSpeed < 0.0f)
                {
                    wallSlideSpeed -= Time.deltaTime;
                }
                else
                {
                    wallSlideSpeed = 0.0f;
                }
            }

            if (jumpCharger < maxJump)
            {
                jumpCharger += Time.deltaTime * 10f;
            }
            else
            {
                jumpCharger = maxJump;
            }
        }
    }

    private void ApplyMovement()
    {
        if(isGrounded && !isInFightStance)
        {
            playerRigidbody.velocity = new Vector2(movementSpeed * movementInputDir, playerRigidbody.velocity.y);
        }
        else if(!isGrounded && !isWallSliding && movementInputDir != 0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDir, 0);
            playerRigidbody.AddForce(forceToAdd);

            if(Mathf.Abs(playerRigidbody.velocity.x) > movementSpeed)
            {
                if(isFacingRight)
                {
                    playerRigidbody.velocity = new Vector2(movementSpeed * Mathf.Abs(movementInputDir), playerRigidbody.velocity.y);
                }
                if(!isFacingRight)
                {
                    playerRigidbody.velocity = new Vector2(movementSpeed * -(Mathf.Abs(movementInputDir)), playerRigidbody.velocity.y);
                }
                
            }
        }
        else if(!isGrounded && !isWallSliding && movementInputDir == 0)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x * airDragMultiplier, playerRigidbody.velocity.y);
        }
       else if(isInFightStance /*&& !isChargingAttack && !isAttacking*/ && !isChargeJumping)
        {
            playerRigidbody.velocity = new Vector2(fightStanceMovementSpeed * movementInputDir, playerRigidbody.velocity.y);
        }
        //else if(isChargingAttack || isAttacking)
        //{
        //    playerRigidbody.velocity = new Vector2(0, 0);
        //}
     

        if (isWallSliding)
        {
            if(playerRigidbody.velocity.y < -wallSlideSpeed)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void CheckMovementDir()
    {
        if(isFacingRight && playerRigidbody.velocity.x < 0 && movementInputDir < 0)
        {
            Flip();
        }
        else if(!isFacingRight && playerRigidbody.velocity.x > 0 && movementInputDir > 0)
        {
            Flip();
        }

        if(playerRigidbody.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void Flip()
    {
        if(!isWallSliding && !isInFightStance)
        {
            facingDir *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
