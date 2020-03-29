using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] private float speed;
    [SerializeField]private float lerpDuration;

    private Quaternion rotateTo;
    private Quaternion initialRotation;
    private float timeToLerp;
    private float timeToLerpBack;

    public bool playerAttacking;
    public int playerHealth;

    [HideInInspector] public bool gameOn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerAttacking = false;
        gameOn = true;
    }

    void Update()
    {
        if (gameOn)
        {
            MoveCharacter();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                timeToLerp = 0f;
                timeToLerpBack = 0f;
                rotateTo = this.transform.rotation;
                initialRotation = this.transform.rotation;
                rotateTo *= Quaternion.Euler(0f, -90f, 0f);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                playerAttacking = true;
                if (timeToLerp < lerpDuration)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, timeToLerp / lerpDuration);
                    timeToLerp += Time.deltaTime;
                    //Debug.Log("lerp time: " + timeToLerp);                    
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(rotateTo, initialRotation, timeToLerpBack / lerpDuration);
                    timeToLerpBack += Time.deltaTime;
                    //Debug.Log("lerp time: " + timeToLerpBack);
                }
            }

            else
            {
                playerAttacking = false;                
                RotateCharacter();
            }
            
        }

        if (playerHealth < 1)
        {
            gameOn = false;
            GameController.Instance.GameOver();
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
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
