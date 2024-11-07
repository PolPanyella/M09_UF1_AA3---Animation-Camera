
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    public GroundDetector gd;
    public CharacterMover cm;
    public Camera cam;
    Animator anim;
    public float rotationScale;
    public Transform gunPivot;
    public Transform gunRightHand;
    public Transform gunLeftHand;

    [Range(0f, 1f)]
    public float lookAtMaxAngle = 0.5f;
    public RaycastLookAt cameraLookAt;
    public float lookAtSpeed = 10;
    Vector3 lookat;
    private void Start()
    {
        anim = GetComponent<Animator>();
        if(cam == null)
        {
            cam = Camera.main;
        }

        lookat = cameraLookAt.lookingAt;
    }
    void Update()
    {
        anim.SetFloat("Sideways", cm.velocity.x);
        anim.SetFloat("Upwards", cm.velocity.y);
        anim.SetFloat("Forward", cm.velocity.magnitude);
        anim.SetFloat("Rotation", cm.velocityAngular * rotationScale);
        anim.SetBool("Grounded", gd.grounded);

        FixLookat();

        gunPivot.LookAt(lookat);
    }

    private void FixLookat()
    {
        lookat = Vector3.Lerp(lookat, cameraLookAt.lookingAt, lookAtSpeed * Time.deltaTime);

        Vector3 forwardLookAt = (lookat - cameraLookAt.transform.position).normalized;
        float dot = Vector3.Dot(forwardLookAt, transform.forward);
        if (dot < lookAtMaxAngle)
        {
            Vector3 axis = Vector3.Cross(forwardLookAt, transform.forward);
            float angle = Vector3.SignedAngle(forwardLookAt, transform.forward, axis);
            float distance = Vector3.Distance(lookat, cameraLookAt.transform.position);
            float maxAngle = Mathf.Acos(lookAtMaxAngle) * Mathf.Rad2Deg;
            forwardLookAt = Quaternion.AngleAxis(angle > 0 ? -maxAngle : maxAngle, axis) * transform.forward;
            lookat = cameraLookAt.transform.position + forwardLookAt * distance;
        }
    }
    private void FixedUpdate()
    {
        Quaternion rot = cm.currentMov.magnitude > 0.01f ? Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cm.currentMov), cm.speedTurn * Time.fixedDeltaTime) : transform.rotation;
        gameObject.transform.rotation = rot;
    }



    void OnAnimatorIK()
    {
        if (anim)
        {

            //if the IK is active, set the position and rotation directly to the goal.


            // Set the look target position, if one has been assigned
            if (gunPivot != null)
            {
                anim.SetLookAtWeight(1, 1, 1);
                anim.SetLookAtPosition(lookat);
                
            }

            // Set the right hand target position and rotation, if one has been assigned
            if (gunRightHand != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKPosition(AvatarIKGoal.RightHand, gunRightHand.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, gunRightHand.rotation);
            }
            if (gunLeftHand != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, gunLeftHand.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, gunLeftHand.rotation);
            }
            {
                
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, gunLeftHand.rotation);

            }
        }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else
        {
            //anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            //anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            //anim.SetLookAtWeight(0);
        }
        
    }
}
