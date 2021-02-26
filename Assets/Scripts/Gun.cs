using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    private Animator animator;
    private AudioSource audio;
    [SerializeField] private AudioClip[] gunfireClips = new AudioClip[4];
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip emptyClip;
    [SerializeField] private ParticleSystem flare;

    [SerializeField] private float damage;

    private float nextFire;
    [SerializeField] private float fireDelay = 0.05f;

    [SerializeField] private float flareDelay = 3f;
    private float nextFlare;
    [SerializeField] private float tracerForce;

    private float maxTotalAmmo = 60;
    private float currentTotalAmmo;
    private float maxClipAmmo = 30;
    private float currentClipAmmo;

    private float maxFlareAmmo = 7;
    private float currentFlareAmmo;
    private float lastFlareRegen;
    private float flareRegenRate = 5f; //TODO balance flare regen rate, or add on kill/pickups as well.

    private int maxTracerRounds = 5;
    private int currentTracerRounds;

    [SerializeField] private Transform tracerOrigin;

    private TextMeshProUGUI _ammoText;
    private TextMeshProUGUI _flareText;

    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        audio = this.GetComponent<AudioSource>();
        currentClipAmmo = maxClipAmmo;
        currentTotalAmmo = maxTotalAmmo;
        currentFlareAmmo = maxFlareAmmo;
        currentTracerRounds = maxTracerRounds;
        cam = Camera.main.transform;
        _flareText = GameManager.Instance.flareText;
        _flareText.text = currentFlareAmmo + "/" + maxFlareAmmo;
        _ammoText = GameManager.Instance.ammoText;
        _ammoText.text = currentClipAmmo + "/" + currentTotalAmmo;
        audio.clip = reloadClip;
        lastFlareRegen = Time.time;
    }

    private void Update()
    {
        RegenerateFlare();
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
        //TODO figure out if I want to use an ammo system for the rifle. Rifle infinite ammo and use special weapons that need to find ammo?
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
        if (Time.time > nextFlare && !isReloading() && canFireFlare())
        {
            if (currentFlareAmmo == maxFlareAmmo) lastFlareRegen = Time.time;
            currentFlareAmmo--;
            _flareText.text = currentFlareAmmo + "/" + maxFlareAmmo;

            ObjectPoolHandler.Instance.CreateTracer(Camera.main.transform.forward, tracerOrigin.position, tracerForce);
            nextFlare = Time.time + flareDelay;
            animator.SetTrigger("FireFlare");
        }
    }
    private bool isReloading()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            return true;
        }
        return false;
    }

    private bool canFireFlare()
    {
        return (currentFlareAmmo - 1 < 0) ? false : true;
    }

    private bool AmmoUpdate()
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

    private void RegenerateFlare()
    {
        if (Time.time > (lastFlareRegen + flareRegenRate))
        {
            if (currentFlareAmmo < maxFlareAmmo)
            {
                currentFlareAmmo++;
                _flareText.text = currentFlareAmmo + "/" + maxFlareAmmo;
                lastFlareRegen = Time.time;
            }
        }
    }

    private void CastRay()
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
