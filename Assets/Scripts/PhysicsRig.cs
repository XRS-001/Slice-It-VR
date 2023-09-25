using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Transform playerHead;
    public float bodyHeightMin = 0.5f;
    public float bodyHeightMax = 2;
    public CapsuleCollider bodyCollider;
    public Transform leftController;
    public Transform rightController;

    public ConfigurableJoint rightJoint;
    public Collider[] rightJointColliders;
    public ConfigurableJoint leftJoint;
    public Collider[] leftJointColliders;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        bodyCollider.height = Mathf.Clamp(playerHead.localPosition.y, bodyHeightMin, bodyHeightMax);
        bodyCollider.center = new Vector3(playerHead.localPosition.x, bodyCollider.height / 2, playerHead.localPosition.z);

        leftJoint.targetPosition = leftController.localPosition;
        leftJoint.targetRotation = leftController.localRotation;

        rightJoint.targetPosition = rightController.localPosition;
        rightJoint.targetRotation = rightController.localRotation;
    }
}
