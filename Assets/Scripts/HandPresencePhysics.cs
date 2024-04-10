using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target;
    public ControllerInteractors controller;
    public RayInteractors ray;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        try
        {
            if(!controller.secondHandGrabbing && !ray.secondHandGrabbing)
            {
                rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;
                rb.MoveRotation(target.rotation);
            }
            else
            {
                rb.velocity = (target.position - transform.position) / Time.fixedDeltaTime;

                Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
                rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);
                Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

                rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
            }
        }
        catch { }
    }
}
