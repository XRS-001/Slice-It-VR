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
        audioSource.pitch = Random.Range(1.1f, 1.4f);
        if(velocity / 7.5f < 0.3f)
        {
            audioSource.PlayOneShot(sliceSound, 0.3f);
        }
        else
        {
            audioSource.PlayOneShot(sliceSound, velocity / 7.5f);
        }
        Vector3 velocitySlice = velocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocitySlice);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if(hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, target.GetComponent<SliceableObject>().slicedMaterial);
            SetupSlicedComponent(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, target.GetComponent<SliceableObject>().slicedMaterial);
            SetupSlicedComponent(lowerHull);

            Destroy(target);
            GameObject.Find("GameManager").GetComponent<GameManager>().score++;
            grabInteractable.controllerGrabbing.SendHapticImpulse(1, 0.05f);
        }
    }
    public void SetupSlicedComponent(GameObject slicedObject)
    {
        slicedObject.layer = 9;
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        rb.mass = 10.0f;
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        Destroy(slicedObject, 5);
    }
}

