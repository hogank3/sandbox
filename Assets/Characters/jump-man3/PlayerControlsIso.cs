using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerControlsIso : MonoBehaviour, PlayerInput.ICharacterControlsActions
{
  #region public
  public float speed;
  public float shootSpeed;
  public float rotationSpeed;
  public float jumpSpeed;
  public float maxJumpHeight;
  public float maxJumpTime;
  public float fireRate = 1.0f;
  public GameObject firePoint;
  public List<GameObject> vfx = new List<GameObject>();
  #endregion

  #region private
  private GameObject effectToSpawn;
  private Animator animator;
  private PlayerInput input;
  private CharacterController characterController;
  private Vector2 movementInput;
  private Vector3 movementTranslate;
  private Vector3 movementDirection;
  private Vector3 shootDirection;
  private float gravity = -6.8f;
  private float groundedGravity = -.5f;
  private float initialJumpVelocity;
  private float secondJumpVelocity;
  private bool isMovePressed = false;
  private bool isShootPressed = false;
  private bool isJumpPressed = false;
  private bool isMoving = false;
  private float timer = 0.0f;

  Camera mainCamera;
//   private bool isJumping = false;
//   private bool isFalling = false;
//   private bool isGrounded = false;
//   private int jumpCount = 0;
//   private bool isFlipping = false;
  private Quaternion rotation = Quaternion.Euler(0, 45.0f, 0);
  private Vector2 shootInput;

  #endregion

  private void Awake()
  {
    input = new PlayerInput();
    input.CharacterControls.Move.started += OnMove;
    input.CharacterControls.Move.performed += OnMove;
    input.CharacterControls.Move.canceled += OnMove;
    input.CharacterControls.Jump.started += OnJump;
    input.CharacterControls.Jump.canceled += OnJump;
    input.CharacterControls.Shoot.started += OnShoot;
    input.CharacterControls.Shoot.performed += OnShoot;
    input.CharacterControls.Shoot.canceled += OnShoot;

    // setJumpVariables();
  }

  public void OnShoot(InputAction.CallbackContext obj)
  {
    shootInput = obj.ReadValue<Vector2>();
    shootDirection.x = shootInput.x;
    shootDirection.z = shootInput.y;
    isShootPressed = shootDirection.x != 0 || shootDirection.z != 0;
    Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(shootInput.x, 0.0f, shootInput.y));
    Debug.Log(worldPos);
  }

  public void OnMove(InputAction.CallbackContext ctx)
  {
    movementInput = ctx.ReadValue<Vector2>();
    movementDirection.x = movementInput.x;
    movementDirection.z = movementInput.y;
    // movementTranslate.x = movementInput.x;
    // movementTranslate.z = movementInput.y;
    // Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
    // movementDirection = isoMatrix.MultiplyPoint3x4(movementTranslate);
    isMovePressed = movementDirection.x != 0 || movementDirection.z != 0;
  }

  public void OnJump(InputAction.CallbackContext ctx)
  {
    isJumpPressed = ctx.ReadValueAsButton();
    Debug.Log(isJumpPressed);
  }

  void Start()
  {
    characterController = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();
    mainCamera = FindObjectOfType<Camera>();
    effectToSpawn = vfx[0];
  }

  void Update()
  {
    handleAnimation();
    handleGravity();
    handleMove();
    handleRotation();
  }

  // void FixedUpdate()
  // {
  //   float rayDistance = 10.0f;
  //   Vector3 origin = transform.position;
  //   origin.y = 0.8f;
  //   Vector3 forward = transform.TransformDirection(Vector3.forward) * rayDistance;
  //   Debug.DrawRay(origin, forward, Color.red);

  //   RaycastHit hit;

  //   if (Physics.Raycast(origin, forward, out hit, rayDistance))
  //   {
  //     //print("Hit - distance: " + hit.distance);
  //     EnemyController enemy = hit.transform.GetComponent<EnemyController>();
  //     if(enemy != null)
  //     {
  //       enemy.TakeDamage(1.0f);
  //     }
  //   }
  // }

  void OnEnable()
  {
    input.CharacterControls.Enable();
  }

  void OnDisable()
  {
    input.CharacterControls.Disable();
  }

  void handleAnimation()
  {
    animator.SetBool("isMoving", isMoving);
    // animator.SetBool("isJumping", isJumping);
    // animator.SetBool("isGrounded", isGrounded);
    // animator.SetBool("isFalling", isFalling);
    // animator.SetBool("isFlipping", isFlipping);
  }

  void handleMove()
  {
    if(isShootPressed)
    {
      characterController.Move(movementDirection * Time.deltaTime * shootSpeed);
    }
    else
    {
      characterController.Move(movementDirection * Time.deltaTime * speed);
    }
    isMoving = (movementDirection.x != 0 || movementDirection.z != 0) ? true : false;
    movementDirection.y += gravity * Time.deltaTime;
  }

  void handleRotation()
  {
    // Vector3 toLookRoation = new Vector3(movementDirection.x, 0, movementDirection.z);
    Vector3 toLookRoation;
    Quaternion currentRotation = transform.rotation;
    
    if(isShootPressed)
    {
      toLookRoation.x = shootDirection.x;
      toLookRoation.y = 0.0f;
      toLookRoation.z = shootDirection.z;

      Quaternion targetRotation = Quaternion.LookRotation(toLookRoation);
      transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);

      timer += Time.deltaTime;

      if(timer > fireRate)
      {
        float rayDistance = 10.0f;
        Vector3 origin = transform.position;
        origin.y = 0.8f;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * rayDistance;
        Debug.DrawRay(origin, forward, Color.red);
  
        RaycastHit hit;
  
        if (Physics.Raycast(origin, forward, out hit, rayDistance))
        {
          //print("Hit - distance: " + hit.distance);
          EnemyController enemy = hit.transform.GetComponent<EnemyController>();
          if(enemy != null)
          {
            enemy.TakeDamage(1.0f);
          }
        }
        timer = 0.0f;

        GameObject vfx;
        if(firePoint != null)
        {
          vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
          vfx.transform.rotation = transform.rotation;
        }
        else
        {
          Debug.Log("No Fire Point");
        }
      }
    }
    else if (isMovePressed)
    {
      toLookRoation.x = movementDirection.x;
      toLookRoation.y = 0.0f;
      toLookRoation.z = movementDirection.z;

      Quaternion targetRotation = Quaternion.LookRotation(toLookRoation);
      transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
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
}
