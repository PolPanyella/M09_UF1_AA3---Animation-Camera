using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericGun : MonoBehaviour
{
    [Header("Weapon")]
    public int clipMax = 30;
    public int clipCurrent = 30;
    public bool automatic = true;
    bool reloading = false;
    [Min(1f/60f)]
    public float fireRate = 0.1f;
    public float reloadTime = 0.5f;
    [Header("Firing")]
    public UnityEvent onFire;
    public Transform firePoint;
    public GameObject bullet;
    [Header("Animation")]
    public float positionRecover;
    public float rotationRecover;
    public Vector3 knockbackPosition;
    public Vector3 knockbackRotation;
    Vector3 originalPosition;
    Quaternion originalRotation;
    float timerFireRate = 0;
    float timerReloadTime = 0;


    [Header("Puntero")]
    public RectTransform transformPointer;
    public LayerMask raycastMask;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        clipCurrent = clipMax;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, positionRecover * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, rotationRecover * Time.deltaTime);
        timerFireRate -= Time.deltaTime;

        if (automatic) {
            if (Input.GetMouseButton(0) && timerFireRate <= 0)
            {
                if (clipCurrent > 0)
                {
                    Fire();
                    timerFireRate = fireRate;
                }
                else
                {
                    if (!reloading)
                    {
                        timerReloadTime = reloadTime;
                        reloading = true;

                    }
                }

            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && timerFireRate <= 0)
            {
                if (clipCurrent > 0)
                {
                    Fire();
                    timerFireRate = fireRate;
                }
                else
                {
                    if (!reloading)
                    {
                        timerReloadTime = reloadTime;
                        reloading = true;

                    }
                }

            }
        }

        if (reloading)
        {
            timerReloadTime -= Time.deltaTime;
        }
        if (timerReloadTime < 0 && reloading) {
            reloading = false;
            clipCurrent = clipMax;

        }



        RaycastHit hit;
        if(Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, 50, raycastMask))
        {

        }
        Debug.DrawRay(firePoint.transform.position, firePoint.transform.position + firePoint.forward * 40, Color.red);


        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(hit.point);  //convert game object position to VievportPoint
        //print(viewportPoint);
        transformPointer.anchoredPosition = new Vector2(1920 * viewportPoint.x, 1080 * viewportPoint.y);

        //transformPointer.localPosition = new Vector3(viewportPoint.x * 100, viewportPoint.y * 100, 0);
    }
    public void Fire()
    {
        Destroy(Instantiate(bullet, firePoint.position, firePoint.rotation), 10);
        onFire.Invoke();
        StartCoroutine(Knockback_Corutine());
        clipCurrent -= 1;
    }
    IEnumerator Knockback_Corutine()
    {
        yield return null;
        transform.localPosition -= new Vector3(Random.Range(-knockbackPosition.x, knockbackPosition.x), Random.Range(0, knockbackPosition.y), Random.Range(-knockbackPosition.z, -knockbackPosition.z * .5f));
        transform.localEulerAngles -= new Vector3(Random.Range(knockbackRotation.x * 0.5f, knockbackRotation.x), Random.Range(-knockbackRotation.y, knockbackRotation.y), Random.Range(-knockbackRotation.z, knockbackRotation.z));
    }
}
