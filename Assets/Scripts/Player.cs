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
    float maxSwayReference;
    public float currentSwaySpeed;
    public float swayBuildUp;
    public float swayTimer;
    float initialZ;
    #endregion

    [SerializeField] private Gun currentGun;
    private GameObject gunGO;

    private AudioSource _audio;

    private float damageTakenCooldown;
    private float regenDelay = 5f;
    private float nextRegen;

    private int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        gunGO = currentGun.gameObject;
        initialZ = gunGO.transform.rotation.eulerAngles.z;
        currentHealth = maxHealth;
        _audio = GetComponent<AudioSource>();
        GameManager.Instance.UpdatePlayerHealth(currentHealth);
        maxSwayReference = maxSwaySpeed;
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
        RegenLife();
    }
    public void TakeDamage(int damage)
    {
        if (currentHealth - damage <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            currentHealth -= damage;
            _audio.Play();
        }
        GameManager.Instance.UpdatePlayerHealth(currentHealth);
    }
    private void Sway(bool moving)
    {

        if (moving)
        {
            if (currentSwaySpeed < maxSwaySpeed && currentSwaySpeed + swayBuildUp <= maxSwaySpeed)
            {
                currentSwaySpeed += swayBuildUp;
            }

            if (currentSwaySpeed > maxSwaySpeed)
            {
                currentSwaySpeed -= swayBuildUp;
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

    private void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.transform.Rotate(Vector3.up * mouseX);
    }

    private void MovePlayer()
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
            maxSwaySpeed = maxSwayReference * 2f;
        }
        else
        {
            move = move * movementSpeed * Time.deltaTime;
            maxSwaySpeed = maxSwayReference;
        }

        controller.Move(move);

        //Simulate gravity
        gravityVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(gravityVelocity * Time.deltaTime);
    }

    private void RegenLife()
    {
        if (Time.time > nextRegen)
        {
            currentHealth++;
            if (currentHealth > 100) currentHealth = 100;
            GameManager.Instance.UpdatePlayerHealth(currentHealth);
            nextRegen = Time.time + regenDelay;
        }

    }

    private void Die()
    {
        GameManager.Instance.SwapToGameOverScene();
        //TODO: implement menu etc.
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("EnemyAttackArea"))
    //    {
    //        if (Time.time > damageTakenCooldown)
    //        {
    //            Debug.Log(Time.time + " is tijd " + " || is cooldown" + damageTakenCooldown);
    //            TakeDamage(10);
    //        }
    //        damageTakenCooldown = Time.time + 12f;
    //    }
    //}
}
