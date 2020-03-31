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
    [SerializeField] private float throwSpeed;
    [SerializeField] private float throwDuration;

    private Quaternion rotateTo;
    private Quaternion initialRotation;
    private float timeToLerp;
    private float timeToLerpBack;
    private Vector3 movingDirection;
    
    private ScytheController scytheController;
    private Vector3 origScytheLocPos;
    private Vector3 origScytheLocRot;
    private float timeToThrowScythe;
    private float timeToReturnScythe;

    public bool playerAttacking;

    public int playerHealth;
    public bool spinUnlocked;
    public bool throwUnlocked;
    public bool chargeUnlocked;

    [HideInInspector] public bool gameOn;
    [HideInInspector] public bool hasScythe;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerAttacking = false;
        hasScythe = true;

        spinUnlocked = false;
        throwUnlocked = false;
        spinUnlocked = false;

        gameOn = true;

        scytheController = scythe.GetComponent<ScytheController>();
        origScytheLocPos = scythe.transform.localPosition;
        origScytheLocRot = scythe.transform.localEulerAngles;
    }

    void Update()
    {
        if (gameOn)
        {            
            if (Input.GetKeyDown(KeyCode.Space) && (hasScythe == true))
            {
                timeToLerp = 0f;
                timeToLerpBack = 0f;
                rotateTo = this.transform.rotation;
                initialRotation = this.transform.rotation;
                rotateTo *= Quaternion.Euler(0f, -160f, 0f);
            }

            else if (Input.GetKey(KeyCode.Space) && (hasScythe == true))
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

            else if (Input.GetKey(KeyCode.Z) && spinUnlocked && (hasScythe == true))
            {
                playerAttacking = true;
                transform.RotateAround(transform.position, -transform.up, Time.deltaTime * spinSpeed);               
            }

            else if (Input.GetKeyDown(KeyCode.C) && chargeUnlocked && (hasScythe == true))
            {
                timeToLerp = 0f;
                rotateTo = this.transform.rotation;
                initialRotation = this.transform.rotation;
                rotateTo *= Quaternion.Euler(0f, -90f, 0f);
            }

            else if (Input.GetKey(KeyCode.C) && chargeUnlocked && (hasScythe == true))
            {
                playerAttacking = true;
                if (timeToLerp < lerpDuration)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, timeToLerp / lerpDuration);
                    timeToLerp += Time.deltaTime;                  
                }
                transform.Translate(movingDirection * speed * 4 * Time.deltaTime, Space.World);
            }

            else if (Input.GetKeyDown(KeyCode.X) && throwUnlocked && (hasScythe == true))
            {
                MoveCharacter();
                timeToThrowScythe = 0f;
                timeToReturnScythe = 0f;
                playerAttacking = true;
                hasScythe = false;
                scytheController.activated = true;
                scythe.AddComponent<Rigidbody>();
                scythe.transform.parent = null;
                scythe.transform.Translate(movingDirection * throwSpeed * Time.deltaTime, Space.World);
            }

            else if (throwUnlocked && (timeToThrowScythe < throwDuration) && (hasScythe == false))
            {
                scythe.transform.Translate(movingDirection * throwSpeed * Time.deltaTime, Space.World);
                timeToThrowScythe += Time.deltaTime;
                MoveCharacter();
            }

            else if (throwUnlocked && (timeToReturnScythe < throwDuration) && (hasScythe == false))
            {
                scythe.transform.position = Vector3.Lerp(scythe.transform.position, transform.position, timeToReturnScythe/throwDuration);
                timeToReturnScythe += Time.deltaTime * 4f;
                MoveCharacter();
            }

            else if (throwUnlocked && (timeToReturnScythe >= throwDuration) && (hasScythe == false))
            {
                playerAttacking = false;
                hasScythe = true;
                scythe.transform.parent = this.transform;
                scytheController.activated = false;
                scythe.transform.localPosition = origScytheLocPos;
                scythe.transform.localEulerAngles = origScytheLocRot;
                Destroy(scythe.GetComponent<Rigidbody>());
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
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.X))
            {
                movingDirection = move;
            }            
        }        

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
