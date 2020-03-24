using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float rotationSpeed;

    public bool playerAttacking;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerAttacking = false;
    }

    void Update()
    {
        MoveCharacter();
        RotateCharacter();

        if (Input.GetKey(KeyCode.Space))
        {
            playerAttacking = true;
        }
        else
        {
            playerAttacking = false;
        }
    }

    private void MoveCharacter()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0f, moveZ);

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }

    private void RotateCharacter()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                transform.rotation *= Quaternion.Euler(0, -90, 0);
                //transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation *= Quaternion.Euler(0, -90, 0), Time.time * rotationSpeed);
            }
            else
            {
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
            }
            
        }
    }
}
