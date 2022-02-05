using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    #region public
    public float speed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float maxJumpHeight;
    public float maxJumpTime;
    #endregion

    #region private
    private Animator animator;
    private PlayerInput input;
    private CharacterController characterController;
    private Vector2 movementInput;
    private Vector3 movementDirection;
    private float gravity = -6.8f;
    private float groundedGravity = -.05f;
    private float initialJumpVelocity;
    private float secondJumpVelocity;
    private bool isMovePressed = false;
    private bool isJumpPressed = false;
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isGrounded = false;
    private int jumpCount = 0;
    private bool isFlipping = false;
    private float timer;
    #endregion

    private void Awake()
    {
        input = new PlayerInput();
        input.CharacterControls.Move.started += onMove;
        input.CharacterControls.Move.performed += onMove;
        input.CharacterControls.Move.canceled += onMove;
        input.CharacterControls.Jump.started += onJump;
        input.CharacterControls.Jump.canceled += onJump;

        setJumpVariables();
    }

    void onMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
        movementDirection.x = movementInput.x;
        movementDirection.z = movementInput.y;
        isMovePressed = movementDirection.x != 0 || movementDirection.z != 0;
    }

    void onJump(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
        Debug.Log(isJumpPressed);
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        handleAnimation();
        handleMove();
        handleRotation();
        handleGravity();
        handleJump();
        handleDoubleJump();
    }

    void OnEnable()
    {
        input.CharacterControls.Enable();
    }

    void OnDisable()
    {
        input.CharacterControls.Disable();
    }

    void setJumpVariables()
    {
        //jump variables cannot be zero
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight / Mathf.Pow(timeToApex, 2));
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        secondJumpVelocity = (2.8f * maxJumpHeight) / timeToApex;
    }

    void handleGravity()
    {
        if (characterController.isGrounded)
        {
            movementDirection.y = groundedGravity;
        }
        else
        {
            movementDirection.y += gravity * Time.deltaTime;
        }
    }

    void handleAnimation()
    {
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isFlipping", isFlipping);
    }

    void handleMove()
    {
        characterController.Move(movementDirection * Time.deltaTime * speed);
        isMoving = (movementDirection.x != 0 || movementDirection.z != 0) ? true : false;
    }

    void handleRotation()
    {
        // Vector3 toLookRoation = new Vector3(movementDirection.x, 0, movementDirection.z);
        Vector3 toLookRoation;
        toLookRoation.x = movementDirection.x;
        toLookRoation.y = 0.0f;
        toLookRoation.z = movementDirection.z;

        Quaternion currentRotation = transform.rotation;
        
        if(isMovePressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toLookRoation);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // if(isMovePressed)
        // {
        //     Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        // }
    }

    void handleJump()
    {
        if(characterController.isGrounded && isJumpPressed && jumpCount == 0)
        {
            movementDirection.y = initialJumpVelocity;
            isJumpPressed = false;
            isJumping = true;
            jumpCount = 1; 
        }
        else if(characterController.isGrounded)
        {
            isFalling = false;
            isGrounded = true;
            jumpCount = 0;
        }
        else if(!characterController.isGrounded)
        {
            isFalling = true;
            isJumping = false;
            isGrounded = false;
        }
    }

    void handleDoubleJump()
    {
        if (isJumpPressed && jumpCount == 1)
        {
            movementDirection.y = secondJumpVelocity;
            isFalling = false;
            isFlipping = true;
            jumpCount = 0;
        }
        else
        {
            isFlipping = false;
        }
    }
}
