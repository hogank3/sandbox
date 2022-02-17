using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

  public float health = 50.0f;
  private bool isWalking = true, isDead;
  private Animator animator;
  public Transform player;
  public float speed = 3.0f;
  private GameObject playerObject;
  private Vector2 movement;
  private Vector3 direction;
  private Rigidbody rb;
  void Start()
  {
    animator = GetComponent<Animator>();
    player = GameObject.Find("jumpman-short-rig (3)").GetComponent<Transform>();
    rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    animator.SetBool("isWalking", isWalking);
    animator.SetBool("isDead", isDead);
    direction = player.position - transform.position;
    direction.Normalize();
    movement = direction;
  }

  void FixedUpdate()
  {
    Move();
  }

  private void Move()
  {
    rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
    if(!isDead)
      transform.LookAt(player);
  }

  public void TakeDamage(float damage)
  {
    print("Spider: \"HISSSS\" ");
    health -= damage;
    if (health <= 0.0f)
    {
      Die();
    }
  }

  private void Die()
  {
    isWalking = false;
    isDead = true;
    speed = 0.0f;
  }
}
