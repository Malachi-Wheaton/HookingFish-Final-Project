using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 100f;
    public float detectionRange = 25f;
    public float attackRange = 2f;
    public float moveSpeed = 3.5f;
    public float damageAmount = 10f;
    public float attackRate = 1f;

    private float nextAttackTime = 0f;
    private Transform player;
    private NavMeshAgent agent;
    private PlayerController playerHealth;
    public bool gethit;
    public Animator animator;
    public AudioClip hitmarkSound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);

            if (agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("walking", true);
            }
            else
            {
                animator.SetBool("walking", false);
            }

            if (distance <= attackRange && Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackRate;
            }
        }
        else
        {
            animator.SetBool("walking", false);
        }

        if (gethit)
        {
            agent.SetDestination(player.position);
            animator.SetBool("walking", true);
        }
    }

    void AttackPlayer()
    {
        animator.SetTrigger("attack"); 
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        playerHealth.audioSource.PlayOneShot(hitmarkSound);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
