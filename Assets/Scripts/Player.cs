using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    #region Character controller
    CharacterController controller;
    float xRotation = 0f;

    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float mouseSensitivity = 1f;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundMask;

    Vector3 gravityVelocity;
    bool isGrounded;
    #endregion
    #region CharacterSway
    public float swayIntensity;
    public float minSwaySpeed;
    public float maxSwaySpeed;
    public float currentSwaySpeed;
    public float swayBuildUp;
    public float swayTimer;
    float initialZ;
    #endregion

    [SerializeField] Gun currentGun;
    GameObject gunGO;

    private int maxHealth = 100;
    private int currentHealth;

    float damageTakenCooldown;

    AudioSource _audio;
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        gunGO = currentGun.gameObject;
        initialZ = gunGO.transform.rotation.eulerAngles.z;
        currentHealth = maxHealth;
        _audio = GetComponent<AudioSource>();
        GameManager.Instance.UpdatePlayerHealth(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentGun.Fire();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            currentGun.FireTracer();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGun.StartReload();
        }
        RotatePlayer();
        MovePlayer();
    }

    void Sway(bool moving)
    {

        if (moving)
        {
            if (currentSwaySpeed < maxSwaySpeed && currentSwaySpeed + swayBuildUp <= maxSwaySpeed)
            {
                currentSwaySpeed += swayBuildUp;
            }
        }
        else
        {
            if (currentSwaySpeed > minSwaySpeed && currentSwaySpeed - swayBuildUp >= minSwaySpeed)
            {
                currentSwaySpeed -= swayBuildUp;
            }
        }
        swayTimer += Time.deltaTime * currentSwaySpeed;
        Vector3 currentRota = gunGO.transform.localRotation.eulerAngles;
        currentRota.z = initialZ + swayIntensity * Mathf.Sin(swayTimer);
        gunGO.transform.localRotation = Quaternion.Euler(currentRota);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.transform.Rotate(Vector3.up * mouseX);
    }

    void MovePlayer()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0;// = Vector3.zero;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0 && !Input.GetMouseButton(0))
        {
            Sway(true);
        }
        else {
            Sway(false);
        }

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(0))
        {
            move = move * movementSpeed * 1.5f * Time.deltaTime;
        }
        else
        {
            move = move * movementSpeed * Time.deltaTime;
        }

        controller.Move(move);

        //Simulate gravity
        gravityVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(gravityVelocity * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth - damage <= 0)
        {
            Die();
        }
        else
        {
            currentHealth -= damage;
            _audio.Play();
            GameManager.Instance.UpdatePlayerHealth(currentHealth);
        }
    }

    void Die()
    {
        //TODO: implement menu etc.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttackArea"))
        {
            if (Time.time > damageTakenCooldown)
            {
                TakeDamage(10);
            }
            damageTakenCooldown = Time.time + 1f;
        }
    }
}
