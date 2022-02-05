// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerMovement : MonoBehaviour
// {
//     public float speed;
//     public float rotationSpeed;
//     public float jumpSpeed;

//     private Animator animator;
//     private CharacterController characterController;
//     private float ySpeed;
//     private float originalStepOffset;

//     PlayerInput input;

//     void Awake()
//     {
//         input = new PlayerInput();
//         input.CharacterControls.Move.performed += ctx => Debug.Log(ctx.ReadValueAsObject());
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
//         animator = GetComponent<Animator>();
//         characterController = GetComponent<CharacterController>();
//         originalStepOffset = characterController.stepOffset;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // float horizontalInput = Input.GetAxis("Horizontal");
//         // float verticalInput = Input.GetAxis("Vertical");
//         Vector2 inputVector = input.CharacterControls.Move.ReadValue<Vector2>();

//         // Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
//         Vector3 movementDirection = new Vector3(inputVector.x, 0, inputVector.y);
//         float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
//         // float magnitude = movementDirection.magnitude;
//         // magnitude = Mathf.Clamp01(magnitude);
//         movementDirection.Normalize();

//         ySpeed += Physics.gravity.y * Time.deltaTime;

//         if(characterController.isGrounded)
//         {
//             characterController.stepOffset = originalStepOffset;
//             ySpeed = -0f;

//             // if(Input.GetButtonDown("Jump"))
//             // {
//             //     ySpeed = jumpSpeed;
//             // }
//         }
//         else
//         {
//             characterController.stepOffset = originalStepOffset;
//         }

//         Vector3 velocity = movementDirection * magnitude;
//         velocity.y = ySpeed;

//         // transform.Translate(movementDirection * magnitude * Time.deltaTime, Space.World);
//         // transform.Translate(movementDirection * magnitude * speed * Time.deltaTime, Space.World);
//         // characterController.SimpleMove(movementDirection * magnitude);
//         // characterController.Move(velocity * Time.deltaTime);

//         if(movementDirection != Vector3.zero)
//         {
//             animator.SetBool("IsMoving", true);
//             Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

//             transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
//         }
//         else
//         {
//             animator.SetBool("IsMoving", false);
//         }
//     }
// }
