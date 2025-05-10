using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float fireRate = 0.2f;
    public int ammoCount = 30;
    public int maxAmmo = 30;

    [Header("Weapon Effects")]
    public Transform muzzleFlash;
    public GameObject bulletHolePrefab;
    public AudioSource gunAudio;
    public AudioClip fireSound;
    public AudioClip hitSound;
    public AudioClip reloadSound;

    [Header("Recoil Settings")]
    public Transform recoilCamera;
    public float recoilAmount = 2f;
    public float recoilReturnSpeed = 5f;

    private float nextTimeToFire = 0f;
    private bool isReloading = false;
    private Quaternion originalCameraRotation;
    private Quaternion targetRecoilRotation;

    public GameObject hitmark;
    public CamRecoil camRecoil;

    public TextMeshProUGUI ammoTextTMP;

    void Start()
    {
        if (recoilCamera == null && Camera.main != null)
            recoilCamera = Camera.main.transform;

        originalCameraRotation = recoilCamera.localRotation;
        targetRecoilRotation = originalCameraRotation;
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && ammoCount > 0)
        {
            FireWeapon();
            camRecoil.Fire();
        }

        if (Input.GetKeyDown(KeyCode.R) && ammoCount < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        recoilCamera.localRotation = Quaternion.Slerp(recoilCamera.localRotation, targetRecoilRotation, Time.deltaTime * recoilReturnSpeed);
        targetRecoilRotation = Quaternion.Slerp(targetRecoilRotation, originalCameraRotation, Time.deltaTime * recoilReturnSpeed);
        if (ammoTextTMP != null)
            ammoTextTMP.text = $"Ammo: {ammoCount} / {maxAmmo}";
    }

    void FireWeapon()
    {
        ammoCount--;
        nextTimeToFire = Time.time + fireRate;

        if (gunAudio && fireSound)
            gunAudio.PlayOneShot(fireSound);

        if (muzzleFlash)
        {
            muzzleFlash.gameObject.SetActive(true);
            Invoke("HideMuzzleFlash", 0.05f);
        }
        targetRecoilRotation *= Quaternion.Euler(-recoilAmount, 0f, 0f);

        RaycastHit hit;
        if (Physics.Raycast(recoilCamera.position, recoilCamera.forward, out hit, 500f))
        {
            if (bulletHolePrefab)
                Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hitmark != null)
                {                   
                    hitmark.SetActive(true);                   
                    StartCoroutine(HideHitmarkAfterDelay(0.1f));                  
                }

                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(20);
                enemy.gethit = true;
            }
        }
    }
    IEnumerator HideHitmarkAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hitmark != null)
            hitmark.SetActive(false);
    }

    void HideMuzzleFlash()
    {
        if (muzzleFlash)
            muzzleFlash.gameObject.SetActive(false);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        gunAudio.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(2f);
        ammoCount = maxAmmo;
        isReloading = false;
    }
}
