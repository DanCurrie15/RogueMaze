using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public GameObject scythe;

    [SerializeField] private float speed;
    [SerializeField] private float lerpDuration;
    [SerializeField] private float spinSpeed;

    private Quaternion rotateTo;
    private Quaternion initialRotation;
    private float timeToLerp;
    private float timeToLerpBack;
    private Vector3 movingDirection;

    public bool playerAttacking;

    public int playerHealth;
    public bool spinUnlocked;
    public bool throwUnlocked;
    public bool chargeUnlocked;

    [HideInInspector] public bool gameOn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerAttacking = false;

        spinUnlocked = false;
        throwUnlocked = false;
        spinUnlocked = false;

        gameOn = true;
    }

    void Update()
    {
        if (gameOn)
        {            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                timeToLerp = 0f;
                timeToLerpBack = 0f;
                rotateTo = this.transform.rotation;
                initialRotation = this.transform.rotation;
                rotateTo *= Quaternion.Euler(0f, -160f, 0f);
            }

            else if (Input.GetKey(KeyCode.Space))
            {
                playerAttacking = true;
                if (timeToLerp < lerpDuration)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, timeToLerp / lerpDuration);
                    timeToLerp += Time.deltaTime;                  
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(rotateTo, initialRotation, timeToLerpBack / lerpDuration);
                    timeToLerpBack += Time.deltaTime;
                }
            }

            else if (Input.GetKey(KeyCode.Z) && spinUnlocked)
            {
                playerAttacking = true;
                transform.RotateAround(transform.position, -transform.up, Time.deltaTime * spinSpeed);               
            }

            else if (Input.GetKeyDown(KeyCode.C) && chargeUnlocked)
            {
                timeToLerp = 0f;
                rotateTo = this.transform.rotation;
                initialRotation = this.transform.rotation;
                rotateTo *= Quaternion.Euler(0f, -90f, 0f);
            }

            else if (Input.GetKey(KeyCode.C) && chargeUnlocked)
            {
                playerAttacking = true;
                if (timeToLerp < lerpDuration)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, timeToLerp / lerpDuration);
                    timeToLerp += Time.deltaTime;                  
                }
                transform.Translate(movingDirection * speed * 4 * Time.deltaTime, Space.World);
            }

            else if (Input.GetKeyDown(KeyCode.X) && throwUnlocked)
            {
                playerAttacking = true;
            }

            else if (Input.GetKeyUp(KeyCode.X) && throwUnlocked)
            {

            }

            else
            {
                playerAttacking = false;                
                MoveCharacter();
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
        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
            movingDirection = move;
        }        

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
