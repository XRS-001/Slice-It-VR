using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    public Transform leftController;
    public Transform rightController;

    public ConfigurableJoint rightJoint;
    public Collider[] rightJointColliders;
    public ConfigurableJoint leftJoint;
    public Collider[] leftJointColliders;

    public float distanceLeft;
    public float distanceRight;
    public float threshold = 0.5f;
    // Update is called once per frame
    void FixedUpdate()
    {

        leftJoint.targetPosition = leftController.localPosition;
        leftJoint.targetRotation = leftController.localRotation;

        rightJoint.targetPosition = rightController.localPosition;
        rightJoint.targetRotation = rightController.localRotation;
    }
}
