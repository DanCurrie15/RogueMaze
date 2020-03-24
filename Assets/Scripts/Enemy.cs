using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            Debug.Log("colliding with: " + collision.GetContact(i).otherCollider.gameObject.tag);
            if (collision.GetContact(i).otherCollider.gameObject.CompareTag("scythe") && Player.Instance.playerAttacking == true)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        
    }
}
