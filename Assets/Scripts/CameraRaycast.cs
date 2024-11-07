using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    public LayerMask maskRaycast;
    public GameObject pivot;
    public float distanceCamera = 10;
    float lerpCam = 0;
    float distanceLerpInitial = 0;
    bool canPutDistance = false;
    void Start()
    {
        transform.localPosition = new Vector3(0, 0, -distanceCamera);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.SphereCast(pivot.transform.position, 0.1f, (transform.position - pivot.transform.position).normalized, out hit, distanceCamera, maskRaycast))
        {
            transform.localPosition = new Vector3(0,0, -hit.distance + 0.1f);
            canPutDistance = true;
            lerpCam = 0;
        }
        else
        {
            if (canPutDistance)
            {
                canPutDistance = false;
                distanceLerpInitial = transform.localPosition.z;
            }
            lerpCam += Time.fixedDeltaTime / 2;
            float distanceToPut = Mathf.Lerp(transform.localPosition.z, -distanceCamera, lerpCam);
            transform.localPosition = new Vector3(0, 0, distanceToPut);
        }
    }
}
