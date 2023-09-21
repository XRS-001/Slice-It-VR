using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerInteractors : XRDirectInteractor
{
    public float weight;
    public HandData handRig;
    public GameObject[] colliders;
    private Rigidbody rb;
    private Transform attach;
    public GameObject handPresence;
    private ConfigurableJoint joint;
    public bool isGrabbing;
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        isGrabbing = true;
        if (args.interactableObject is XRGrabInteractableTwoAttach)
        {
            rb = args.interactableObject.transform.GetComponent<Rigidbody>();
            weight = rb.mass;
            rb.mass = 0.1f;
            attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
            StartCoroutine(DelayEnter());
            joint = handPresence.AddComponent<ConfigurableJoint>();
            joint.enableCollision = false;
            joint.connectedBody = rb;

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.massScale = 1f;
            joint.connectedMassScale = 1f;
            base.OnSelectEntered(args);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        rb.mass = weight;
        weight = 0;
        handPresence.transform.position = transform.position;
        handPresence.transform.rotation = transform.rotation;

        if (args.interactableObject is XRGrabInteractableTwoAttach)
        {
            DestroyJoint();
            base.OnSelectExited(args);
        }
        StartCoroutine(DelayExit());
    }
    public void DestroyJoint()
    {
        joint.connectedBody.useGravity = true;
        joint.connectedBody = null;
        Destroy(joint);
    }
    public void ReleaseInteractable()
    {
        if (selectTarget != null)
        {
            enabled = false;
            enabled = true;
        }
    }
    public IEnumerator DelayEnter()
    {
        foreach (Collider collider in handPresence.GetComponent<HandPresencePhysics>().handColliders)
        {
            collider.isTrigger = true;
        }
        handPresence.transform.position = attach.position;
        handPresence.transform.rotation = attach.rotation;
        yield return new WaitForSeconds(1f);

        foreach (Collider collider in handPresence.GetComponent<HandPresencePhysics>().handColliders)
        {
            collider.isTrigger = false;
        }
    }
    public IEnumerator DelayExit()
    {
        yield return new WaitForSeconds(0.5f);

        handPresence.GetComponent<HandPresencePhysics>().handColliderParent.SetActive(true);
    }
}

