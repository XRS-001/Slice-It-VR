using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Slice : MonoBehaviour
{
    XRGrabInteractableTwoAttach grabInteractable;
    public AudioSource audioSource;
    public AudioClip sliceSound;
    private float velocity;
    public float speedNeededToSlice;
    private bool hasHit;
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    private bool hasHitAlt;
    public Transform startSlicePointAlt;
    public Transform endSlicePointAlt;
    public VelocityEstimator velocityEstimator;
    public LayerMask sliceableLayer;
    public Material crossSectionMaterial;
    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
        if(startSlicePointAlt != null && endSlicePointAlt != null) 
        {
            hasHitAlt = Physics.Linecast(startSlicePointAlt.position, endSlicePointAlt.position, out RaycastHit hitAlt, sliceableLayer);

            if (hasHitAlt && velocity > speedNeededToSlice)
            {
               GameObject target = hitAlt.transform.gameObject;
               SliceObject(target);
            }
            if (hasHit && velocity > speedNeededToSlice)
            {
                GameObject target = hit.transform.gameObject;
                SliceObject(target);
            }
        }
        else
        {
            if (hasHit && velocity > speedNeededToSlice)
            {
                GameObject target = hit.transform.gameObject;
                SliceObject(target);
            }
        }

    }
    public void SliceObject(GameObject target)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(sliceSound);
        Vector3 velocitySlice = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocitySlice);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull);

            Destroy(target);
            GameObject.Find("GameManager").GetComponent<GameManager>().score++;
            grabInteractable.controllerGrabbing.SendHapticImpulse(1, 0.05f);
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = 10.0f;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        Destroy(slicedObject, 5);
    }
}

