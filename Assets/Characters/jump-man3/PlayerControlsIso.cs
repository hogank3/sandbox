using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerControlsIso : MonoBehaviour, PlayerInput.ICharacterControlsActions
{
  #region public
  public float speed, shootSpeed, rotationSpeed, jumpSpeed, maxJumpHeight, maxJumpTime, fireRate = 1.0f, health = 5.0f;
  public GameObject firePoint;
  public List<GameObject> vfx = new List<GameObject>();
  #endregion

  #region private
  private GameObject _effectToSpawn;
  private Animator _animator;
  private PlayerInput _input;
  private CharacterController _characterController;
  private Vector2 _movementInput;
  private Vector3 _movementTranslate, _movementDirection, _shootDirection;
  private float _gravity = -6.8f, _groundedGravity = -.5f, _initialJumpVelocity, _secondJumpVelocity, _timer = 0.0f;
  private bool _isMovePressed = false, _isShootPressed = false, _isJumpPressed = false, _isMoving = false, _isFiring = false;

  private Camera _mainCamera;
  private Quaternion rotateAmount = Quaternion.Euler(0, 45.0f, 0);
  private Vector2 _shootInput;

  #endregion

  private void Awake()
  {
    _input = new PlayerInput();

    //input move
    _input.CharacterControls.Move.started += OnMove;
    _input.CharacterControls.Move.performed += OnMove;
    _input.CharacterControls.Move.canceled += OnMove;

    //input jump
    _input.CharacterControls.Jump.started += OnJump;
    _input.CharacterControls.Jump.canceled += OnJump;

    //input shoot
    _input.CharacterControls.Shoot.started += OnShoot;
    _input.CharacterControls.Shoot.performed += OnShoot;
    _input.CharacterControls.Shoot.canceled += OnShoot;

    //input puase
    _input.CharacterControls.Pause.started += OnPause;
    // setJumpVariables();
  }

  public void OnPause(InputAction.CallbackContext obj)
  {
    //throw new NotImplementedException();
    //Debug.Log("Pause Pressed");
  }

  public void OnShoot(InputAction.CallbackContext obj)
  {
    _shootInput = obj.ReadValue<Vector2>();
    _shootDirection.x = _shootInput.x;
    _shootDirection.z = _shootInput.y;
    _isShootPressed = _shootDirection.x != 0 || _shootDirection.z != 0;
    //Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(_shootInput.x, 0.0f, _shootInput.y));
    //Debug.Log(worldPos);
  }

  public void OnMove(InputAction.CallbackContext ctx)
  {
    _movementInput = ctx.ReadValue<Vector2>();
    _movementDirection.x = _movementInput.x;
    _movementDirection.z = _movementInput.y;
    // _movementTranslate.x = _movementInput.x;
    // _movementTranslate.z = _movementInput.y;
    // Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotateAmount);
    // _movementDirection = isoMatrix.MultiplyPoint3x4(_movementTranslate);
    _isMovePressed = _movementDirection.x != 0 || _movementDirection.z != 0;
  }

  public void OnJump(InputAction.CallbackContext ctx)
  {
    _isJumpPressed = ctx.ReadValueAsButton();
    Debug.Log(_isJumpPressed);
  }

  void Start()
  {
    _characterController = GetComponent<CharacterController>();
    _animator = GetComponent<Animator>();
    _mainCamera = FindObjectOfType<Camera>();
    _effectToSpawn = vfx[0];
    Physics.IgnoreLayerCollision(2, 2);
  }

  void Update()
  {
    HandleAnimation();
    HandleGravity();
    HandleMove();
    HandleRotation();
  }

  private void HandleDamage()
  {
    health -= 1.0f;
    if(health <= 0.0f)
      print("You Died.");
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
    _input.CharacterControls.Enable();
  }

  void OnDisable()
  {
    _input.CharacterControls.Disable();
  }

  void HandleAnimation()
  {
    float velocityZ = Vector3.Dot(_movementDirection.normalized, transform.forward);
    float velocityX = Vector3.Dot(_movementDirection.normalized, transform.right);

    _animator.SetFloat("velocityZ", velocityZ, 0.1f, Time.deltaTime);
    _animator.SetFloat("velocityX", velocityX, 0.1f, Time.deltaTime);

    _animator.SetBool("isMoving", _isMoving);
    _animator.SetBool("isFiring", _isFiring);
    // _animator.SetBool("isJumping", isJumping);
    // _animator.SetBool("isGrounded", isGrounded);
    // _animator.SetBool("isFalling", isFalling);
    // _animator.SetBool("isFlipping", isFlipping);
  }

  void HandleMove()
  {
    if (_isShootPressed)
    {
      _characterController.Move(_movementDirection * Time.deltaTime * shootSpeed);
    }
    else
    {
      _characterController.Move(_movementDirection * Time.deltaTime * speed);
    }
    _isMoving = (_movementDirection.x != 0 || _movementDirection.z != 0) ? true : false;
    _movementDirection.y += _gravity * Time.deltaTime;
  }

  void HandleRotation()
  {
    // Vector3 toLookRoation = new Vector3(_movementDirection.x, 0, _movementDirection.z);
    Vector3 toLookRoation;
    Quaternion currentRotation = transform.rotation;

    if (_isShootPressed)
    {
      _isFiring = true;

      toLookRoation.x = _shootDirection.x;
      toLookRoation.y = 0.0f;
      toLookRoation.z = _shootDirection.z;

      Quaternion targetRotation = Quaternion.LookRotation(toLookRoation);
      transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);

      _timer += Time.deltaTime;

      if (_timer > fireRate)
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
          if (enemy != null)
          {
            enemy.TakeDamage(1.0f);
          }
        }
        _timer = 0.0f;

        GameObject vfx;
        if (firePoint != null)
        {
          vfx = Instantiate(_effectToSpawn, firePoint.transform.position, Quaternion.identity);
          vfx.transform.rotation = transform.rotation;
        }
        else
        {
          Debug.Log("No Fire Point");
        }
      }
    }
    else if (_isMovePressed)
    {
      _isFiring = false;

      toLookRoation.x = _movementDirection.x;
      toLookRoation.y = 0.0f;
      toLookRoation.z = _movementDirection.z;

      Quaternion targetRotation = Quaternion.LookRotation(toLookRoation);
      transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    else
      _isFiring = false;
  }

  void HandleGravity()
  {
    if (_characterController.isGrounded)
    {
      _movementDirection.y = _groundedGravity;
    }
    else
    {
      _movementDirection.y += _gravity * Time.deltaTime;
    }
  }

  void OnControllerColliderHit(ControllerColliderHit colliderHit)
  {
    if(colliderHit.gameObject.tag == "Enemy")
    {
      HandleDamage();
      print("collisoin detected");
      //transform.position = transform.position + (transform.position - colliderHit.transform.position);
    }
  }
}
