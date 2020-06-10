using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    PATROL,
    CHASE,
    ATTACK
}
public class EnemyController : MonoBehaviour
{
    private EnemyAnimator enemyAnim;
    private NavMeshAgent navAgent;

    public EnemyState enemyState;
    
    public float walkSpeed = 0.5f;
    public float runSpeed = 4f;

    public float chaseDistance = 20f;
    public float retreatDistance = 1.5f;

    private float currentChaseDistance;
    public float attackDistance = 10f;
    public float chaseAfterAttackDistance = 2f;

    public float patrolRadiusMin = 20f, patrolRadiusMax = 60f;
    public float patrolForThisTime = 15f;
    private float patrolTimer;

    public float waitBeforeAttack = 2f;
    private float attackTimer;

    private Transform target;

    private void Awake()
    {
        enemyAnim = GetComponent<EnemyAnimator>();
        navAgent = GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        enemyState = EnemyState.PATROL;
        patrolTimer = patrolForThisTime;
        attackTimer = waitBeforeAttack;
        currentChaseDistance = chaseDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.PATROL)
        {
            Patrol();
        }

        if (enemyState == EnemyState.CHASE)
        {
            Chase();
        }

        if (enemyState == EnemyState.ATTACK)
        {
            Attack();
        }
    }

    void Patrol()
    {

        // tell nav agent that he can move
        navAgent.isStopped = false;
        navAgent.speed = runSpeed;

        // add to the patrol timer
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolForThisTime)
        {
            SetNewRandomDestination();
            patrolTimer = 0f;
        }

        if (navAgent.velocity.sqrMagnitude > 0)
        {
            enemyAnim.Run(true);
        }
        else
        {
            enemyAnim.Run(false);
        }

        // test the distance between the player and the enemy
        if (Vector3.Distance(transform.position, target.position) <= chaseDistance)
        {
            enemyAnim.Run(false);
            enemyState = EnemyState.CHASE;

            // play spotted audio
            //enemyAudio.Play_ScreamSound();
        }
    }


    void Chase()
    {

        // enable the agent to move again
        navAgent.isStopped = false;
        navAgent.speed = runSpeed;

        // set the player's position as the destination
        // because we are chasing(running towards) the player
        navAgent.SetDestination(target.position);

        if (navAgent.velocity.sqrMagnitude > 0)
        {
            enemyAnim.Run(true);
        }
        else
        {
            enemyAnim.Run(false);
        }

        // if the distance between enemy and player is less than attack distance
        if (Vector3.Distance(transform.position, target.position) <= attackDistance)
        {

            // stop the animations
            enemyAnim.Run(false);
            navAgent.velocity = Vector3.zero;
            navAgent.isStopped = true;
            enemyState = EnemyState.ATTACK;

            // reset the chase distance to previous
            if (chaseDistance != currentChaseDistance)
            {
                chaseDistance = currentChaseDistance;
            }

        }
        else if (Vector3.Distance(transform.position, target.position) > chaseDistance)
        {
            // player run away from enemy

            // stop running
            enemyAnim.Run(false);

            enemyState = EnemyState.PATROL;

            // reset the patrol timer so that the function
            // can calculate the new patrol destination right away
            patrolTimer = patrolForThisTime;

            // reset the chase distance to previous
            if (chaseDistance != currentChaseDistance)
            {
                chaseDistance = currentChaseDistance;
            }


        } // else

    } // chase

    void Attack()
    {

        navAgent.velocity = Vector3.zero;
        navAgent.isStopped = true;

        attackTimer += Time.deltaTime;

        if (attackTimer > waitBeforeAttack)
        {

            enemyAnim.Attack();
            attackTimer = 0f;

            // play attack sound
            //enemy_Audio.Play_AttackSound();

        }

        if (Vector3.Distance(transform.position, target.position) > attackDistance + chaseAfterAttackDistance)
        {
            enemyState = EnemyState.CHASE;
        }


    } // attack

    void SetNewRandomDestination()
    {

        float rand_Radius = Random.Range(patrolRadiusMin, patrolRadiusMax);

        Vector3 randDir = Random.insideUnitSphere * rand_Radius;
        randDir += transform.position;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDir, out navHit, rand_Radius, -1);

        navAgent.SetDestination(navHit.position);

    }
}
