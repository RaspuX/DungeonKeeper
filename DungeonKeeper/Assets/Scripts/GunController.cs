using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunController : MonoBehaviour
{
    [Header("Gun Settings")]
    public float timeBetweenShooting, range, reloadTime, timeBetweenShots;
    public int clipSize, bulletsPerTap;
    public int reservedAmmoCapacity = 270;
    public bool allowButtonHold;
    public Transform attackPoint;

    [Header("Visuals")]
    public Image muzzleFlashImage;
    public Sprite[] flashes;
    public TextMeshProUGUI AmmoCountText;
    public GameObject bulletHoleGraphic;

    [Header("Targets")]
    public LayerMask whatIsEnemy;
    public LayerMask Weapon;

    [Header("Aiming")]
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float aimSmoothing = 10;
    public bool aiming = false;

    [Header("Mouse Settings")]
    [SerializeField] private float weaponSwayAmount;
    public float currentWeaponSwayAmount;
    public MouseLook cam;

    [Header("Weapon Recoil")]
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    public Vector2[] recoilPattern;

    [Header("Spread")]
    public float spread;

    private bool shooting, canShoot, reloading;
    private int currentAmmoInClip, ammoInReserve;
    private int bulletsShot;

    private void Start()
    {
        currentAmmoInClip = clipSize;
        ammoInReserve = reservedAmmoCapacity;
        canShoot = true;

    }

    private void Update()
    {
        DetermineAim();
        DetermineRotation();
        HandleReload();
        MyInput();
        AmmoCountText.SetText(currentAmmoInClip + " / " + ammoInReserve);

        if (aiming)
        {
            currentWeaponSwayAmount = weaponSwayAmount / 10;
        }
        else
        {
            currentWeaponSwayAmount = weaponSwayAmount;
        }

        if (Input.GetMouseButton(1))
        {
            aiming = true;
        }
        else
        {
            aiming = false;
        }

        if (shooting && canShoot && !reloading && currentAmmoInClip > 0)
        {
            Shoot();
        }
    }


    private void MyInput()
    {
        shooting = allowButtonHold ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        if (canShoot && shooting && !reloading && currentAmmoInClip > 0)
        {
            // Determine the number of bullets to shoot based on bulletsPerTap
            bulletsShot = Mathf.Min(bulletsPerTap, currentAmmoInClip);
            for (int i = 0; i < bulletsShot; i++)
            {
                Shoot();
            }
        }
    }


    void Shoot()
    {
        canShoot = false;
        currentAmmoInClip--;

        StartCoroutine(ShootGun());
    }

    IEnumerator ShootGun()
    {
        DetermineRecoil();
        StartCoroutine(MuzzleFlash());

        // Shoot from the attackPoint
        Vector3 attackPointPosition = attackPoint.position;
        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = attackPoint.transform.forward + new Vector3(x, y, 0);
        Debug.DrawRay(attackPoint.position, direction * range, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(attackPointPosition, direction, out hit, range))
        {
            // Calculate offset position for the bullet hole
            Vector3 bulletHolePosition = hit.point + hit.normal * 0.001f;
            Instantiate(bulletHoleGraphic, bulletHolePosition, Quaternion.LookRotation(hit.normal));
            Debug.Log(hit.collider.name);

            if ((whatIsEnemy & (1 << hit.collider.gameObject.layer)) != 0)
            {
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.None;
                    rb.AddForce(attackPoint.forward * 500);
                }
            }
        }

        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    void ReloadGun()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished()
    {
        int amountNeeded = clipSize - currentAmmoInClip;
        int amountToReload = Mathf.Min(amountNeeded, ammoInReserve);
        currentAmmoInClip += amountToReload;
        ammoInReserve -= amountToReload;
        reloading = false;
        canShoot = true;
    }

    void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmoInClip < clipSize && ammoInReserve > 0 && !reloading)
        {
            ReloadGun();
        }
    }

    void DetermineRotation()
    {
        cam.currentRotation += new Vector2(cam.mouseDelta.x, -cam.mouseDelta.y);
        cam.currentRotation.y = Mathf.Clamp(cam.currentRotation.y, -90, 90);
        transform.localPosition += (Vector3)cam.mouseDelta * currentWeaponSwayAmount / 10000;
        
    }

    void DetermineAim()
    {
        Vector3 targetPosition = Input.GetMouseButton(1) ? aimingLocalPosition : normalLocalPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSmoothing);
    }

    void DetermineRecoil()
    {
        if (randomizeRecoil)
        {
            Vector2 recoil = new Vector2(Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x), Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y));
            cam.currentRotation += recoil;
        }
        else
        {
            int currentStep = clipSize + 1 - currentAmmoInClip;
            currentStep = Mathf.Clamp(currentStep, 0, recoilPattern.Length - 1);
            cam.currentRotation += recoilPattern[currentStep];
        }
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlashImage.sprite = flashes[Random.Range(0, flashes.Length)];
        muzzleFlashImage.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        muzzleFlashImage.color = new Color(0, 0, 0, 0);
        muzzleFlashImage.sprite = null;
    }
}