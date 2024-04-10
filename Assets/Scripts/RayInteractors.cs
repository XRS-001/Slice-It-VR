using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class RayInteractors : XRRayInteractor
{
    private AudioSource audioSource;
    public float weight;
    public HandData handRig;
    public GameObject[] colliders;
    private Rigidbody rb;
    private Transform attach;
    public GameObject handPhysics;
    private ConfigurableJoint joint;
    private ConfigurableJoint configJoint;
    public bool secondHandGrabbing;
    public bool isGrabbing;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        audioSource.pitch = 1.3f;
        audioSource.Play();
        isGrabbing = true;
        if (args.interactableObject is XRGrabInteractableTwoAttach
            || args.interactableObject is TwoHandInteractable)
        {
            rb = args.interactableObject.transform.GetComponent<Rigidbody>();
            weight = rb.mass;
            handPhysics.GetComponent<Rigidbody>().mass = rb.mass;
            handPhysics.GetComponent<Rigidbody>().drag = rb.mass * 3;
            attach = args.interactableObject.transform.GetComponent<XRGrabInteractable>().attachTransform.transform;
            StartCoroutine(DelayEnter());

            configJoint = handPhysics.AddComponent<ConfigurableJoint>();
            configJoint.xMotion = ConfigurableJointMotion.Locked;
            configJoint.yMotion = ConfigurableJointMotion.Locked;
            configJoint.zMotion = ConfigurableJointMotion.Locked;

            configJoint.angularXMotion = ConfigurableJointMotion.Locked;
            configJoint.angularYMotion = ConfigurableJointMotion.Locked;
            configJoint.angularZMotion = ConfigurableJointMotion.Locked;
            configJoint.connectedBody = rb;
            configJoint.autoConfigureConnectedAnchor = false;
            configJoint.connectedAnchor = rb.transform.InverseTransformPoint(handPhysics.transform.position);
            maxRaycastDistance = 0;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        maxRaycastDistance = 30;
        audioSource.pitch = 1f;
        audioSource.Play();
        if (args.interactableObject is XRGrabInteractableTwoAttach
            || args.interactableObject is TwoHandInteractable)
        {
            weight = 0;
            handPhysics.transform.position = transform.position;
            handPhysics.transform.rotation = transform.rotation;
            Destroy(configJoint);
            handPhysics.GetComponent<Rigidbody>().mass = 1;
        }
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
        handPhysics.transform.position = attach.position;
        handPhysics.transform.rotation = attach.rotation;
        yield return new WaitForSeconds(0f);
    }
}