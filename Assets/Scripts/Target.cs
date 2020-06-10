using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    public float health = 50f;
    //public bool isPlayer, isEnemy;
    
    private bool isDead;

    private EnemyAnimator enemyAnim;
    private NavMeshAgent navAgent;
    private EnemyController enemyController;

    //private PlayerStats playerStats;

    void Awake()
    {


        enemyAnim = GetComponent<EnemyAnimator>();
        enemyController = GetComponent<EnemyController>();
        navAgent = GetComponent<NavMeshAgent>();
        
        /*if (isPlayer)
        {
            playerStats = GetComponent<PlayerStats>();
        }*/

    }
    public void TakeDamage(float amuount)
    {
        if (enemyController.enemyState == EnemyState.PATROL)
        {
            enemyController.chaseDistance = 50f;
        }

        if (isDead)
            return;

        health -= amuount;
        if(health <= 0f)
        {
            Die();
            isDead = true;
        }
    }

    void Die()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<BoxCollider>().isTrigger = false;
        GetComponent<Rigidbody>().AddTorque(-transform.forward * 5f);

        enemyController.enabled = false;
        navAgent.enabled = false;
        enemyAnim.enabled = false;
        Destroy(gameObject);
    }
}
