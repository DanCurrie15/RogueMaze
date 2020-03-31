using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float wanderRadius;
    public float wanderTimer;

    public ParticleSystem deathParticles;

    private NavMeshAgent agent;
    private float timer;
    private float searchRadius;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        searchRadius = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius, LayerMask.GetMask("Player"));
        if (hitColliders.Length > 0)
        {
            agent.SetDestination(hitColliders[0].gameObject.transform.position);
            return;
        }

        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }

        
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {        
            //Debug.Log("colliding with: " + collision.GetContact(i).otherCollider.gameObject.tag + " - is player attacking: " +Player.Instance.playerAttacking);
            if (collision.GetContact(i).otherCollider.gameObject.CompareTag("scythe") && Player.Instance.playerAttacking == true)
            {
                //Debug.Log("Enemy hit");
                Destroy(this.gameObject);
                return;
            }
            else if (collision.GetContact(i).otherCollider.gameObject.CompareTag("Player"))
            {
                //Debug.Log("Enemy attacked");
                Player.Instance.playerHealth--;
                GameController.Instance.UpdateHealthLabel();
                Destroy(this.gameObject);
                return;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(deathParticles, this.gameObject.transform);
    }
}
