using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    Animator animator;
    AudioSource audio;
    [SerializeField]ParticleSystem flare;
    [SerializeField] float damage;

    float nextFire;
    [SerializeField] float fireDelay = 0.05f;

    float nextFlare;
    [SerializeField] float flareDelay = 2f;

    float maxAmmo = 120;
    float maxClipAmmo = 30;
    float currentClipAmmo;

    int maxTracerRounds = 5;
    int currentTracerRounds;
    [SerializeField]int tracerFrequency = 5;

    [SerializeField] Transform tracerOrigin;

    [SerializeField] float tracerForce;

    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        audio = this.GetComponent<AudioSource>();
        currentClipAmmo = maxClipAmmo;
        currentTracerRounds = maxTracerRounds;
        cam = Camera.main.transform;
    }

    public void Fire()
    {
        if (Time.time > nextFire)
        {
            animator.SetTrigger("Fire");
            audio.PlayOneShot(audio.clip);
            flare.Play();
            nextFire = Time.time + fireDelay;
            CastRay();
        }
    }

    public void FireTracer()
    {
        //TODO implement clip system for flare and gunfire.
        if (Time.time > nextFlare)
        {
            ObjectPoolHandler.Instance.CreateTracer(Camera.main.transform.forward, tracerOrigin.position, tracerForce);
            //play sound
            nextFlare = Time.time + flareDelay;
            animator.SetTrigger("FireFlare");
        }

    }

    void CastRay()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit)){
            if (hit.collider.transform.root.CompareTag("Enemy"))
            {
                hit.collider.transform.root.GetComponent<Enemy>().TakeDamage(damage);

                Vector3 incomingVec = hit.point - cam.position;
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
                ObjectPoolHandler.Instance.CreateBlood(reflectVec, hit.point);
                Debug.DrawLine(cam.position, hit.point, Color.red);
                Debug.DrawRay(hit.point, reflectVec, Color.green);
            }

            else
            {
                ObjectPoolHandler.Instance.CreateDecal(hit.normal, hit.point);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
