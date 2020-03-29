using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float wanderRadius;
    public float wanderTimer;

    public GameObject deathParticles;

    private Transform target;
    private NavMeshAgent agent;
    private float timer;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("colliding with: " + collision.GetContact(i).otherCollider.gameObject.tag + " - is player attacking: " +Player.Instance.playerAttacking);
            if (collision.GetContact(i).otherCollider.gameObject.CompareTag("scythe") && Player.Instance.playerAttacking == true)
            {
                Debug.Log("Enemy hit");
                Instantiate(deathParticles, this.gameObject.transform);
                Destroy(this.gameObject);
                return;
            }
            else if (collision.GetContact(i).otherCollider.gameObject.CompareTag("Player") && Player.Instance.playerAttacking == false)
            {
                Debug.Log("Enemy attacked");
                Player.Instance.playerHealth--;
                GameController.Instance.UpdateHealthLabel();
                Instantiate(deathParticles, this.gameObject.transform);
                Destroy(this.gameObject);
                return;
            }
        }
    }
}
