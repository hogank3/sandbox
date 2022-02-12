using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

  public float health = 50.0f;
  public bool isDead;
  private Animator animator;
  // Start is called before the first frame update
  void Start()
  {
    animator = GetComponent<Animator>();
  }

  // Update is called once per frame
  void Update()
  {
    animator.SetBool("isDead", isDead);
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
    isDead = true;
  }
}
