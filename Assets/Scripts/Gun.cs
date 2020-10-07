using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    Animator animator;
    AudioSource audio;
    [SerializeField] AudioClip[] gunfireClips = new AudioClip[4];
    [SerializeField] AudioClip reloadClip;
    [SerializeField] AudioClip emptyClip;
    [SerializeField]ParticleSystem flare;
    [SerializeField] float damage;

    float nextFire;
    [SerializeField] float fireDelay = 0.05f;

    float nextFlare;
    [SerializeField] float flareDelay = 3f;

    float maxTotalAmmo = 60;
    float currentTotalAmmo;
    float maxClipAmmo = 30;
    float currentClipAmmo;

    float maxFlareAmmo = 7;
    float currentFlareAmmo;

    int maxTracerRounds = 5;
    int currentTracerRounds;

    [SerializeField] Transform tracerOrigin;

    [SerializeField] float tracerForce;

    TextMeshProUGUI _ammoText;
    TextMeshProUGUI _flareText;

    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        audio = this.GetComponent<AudioSource>();
        currentClipAmmo = maxClipAmmo;
        currentTotalAmmo = maxTotalAmmo;
        currentTracerRounds = maxTracerRounds;
        cam = Camera.main.transform;
        _ammoText = GameManager.Instance.ammoText;
        _ammoText.text = currentClipAmmo + "/" + currentTotalAmmo;
        currentFlareAmmo = maxFlareAmmo;
        audio.clip = reloadClip;
    }

    public void Fire()
    {
        if (Time.time > nextFire)
        {
            if (!isReloading())
            {
                if (AmmoUpdate())
                {
                    animator.SetTrigger("Fire");
                    audio.PlayOneShot(gunfireClips[Random.Range(0, gunfireClips.Length)]);
                    flare.Play();
                    CastRay();
                }
                else
                {
                    animator.SetTrigger("FireEmpty");
                }
            }
            nextFire = Time.time + fireDelay;

        }
    }

    bool isReloading()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            return true;
        }
        return false;
    }

    bool AmmoUpdate()
    {
        if (currentClipAmmo - 1 < 0)
        {
            return false;
        }
        else
        {
            currentClipAmmo--;
            if (currentClipAmmo < 10)
            {
                _ammoText.text = "0" + currentClipAmmo + "/" + currentTotalAmmo;
            }
            else
            {
                _ammoText.text = currentClipAmmo + "/" + currentTotalAmmo;
            }

            return true;
        }
    }

    public void StartReload()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Reload") && currentClipAmmo != maxClipAmmo && currentTotalAmmo != 0)
        {
            animator.SetTrigger("Reload");
            audio.Play();
        }
    }

    public void PlayEmptySound()
    {
        audio.PlayOneShot(emptyClip);
    }

    public void Reload()
    {
        currentClipAmmo = maxClipAmmo;
        currentTotalAmmo = maxTotalAmmo;
        _ammoText.text = currentClipAmmo + "/" + currentTotalAmmo;

        return;
        //TODO Create ammo drops?
        float amountToReload = maxClipAmmo - currentClipAmmo;

        if (currentTotalAmmo - amountToReload >= 0)
        {
            currentClipAmmo = maxClipAmmo;
            currentTotalAmmo -= amountToReload;
        }

        else if (currentTotalAmmo - amountToReload < 0)
        {
            currentClipAmmo = currentTotalAmmo;
            currentTotalAmmo = 0;
        }
        _ammoText.text = currentClipAmmo + "/" + currentTotalAmmo;
    }

    public void FireTracer()
    {
        //TODO implement clip system for flare and gunfire.
        if (Time.time > nextFlare && !isReloading())
        {
            ObjectPoolHandler.Instance.CreateTracer(Camera.main.transform.forward, tracerOrigin.position, tracerForce);
            nextFlare = Time.time + flareDelay;
            animator.SetTrigger("FireFlare");
        }

    }

    void CastRay()
    {
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit)){
            if (hit.collider.transform.root.CompareTag("Enemy")) //TOOD: refactor collision detection
            {
                hit.collider.transform.root.GetComponent<Enemy>().TakeDamage(damage);

                Vector3 incomingVec = hit.point - cam.position;
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
                ObjectPoolHandler.Instance.CreateBlood(reflectVec, hit.point);
                //Debug.DrawLine(cam.position, hit.point, Color.red);
                //Debug.DrawRay(hit.point, reflectVec, Color.green);
            }

            else if (hit.collider.transform.root.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                ObjectPoolHandler.Instance.CreateDecal(hit.normal, hit.point);
            }
        }
    }
}
