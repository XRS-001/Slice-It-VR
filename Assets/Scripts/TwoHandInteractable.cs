using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandInteractable : XRGrabInteractableTwoAttach
{
    public Collider secondHandGrabPointCollider;
    public Transform rightAttachSecond;
    public Transform leftAttachSecond;
    public XRSimpleInteractable secondHandGrabPoint;
    private XRBaseInteractor secondInteractor;
    private Quaternion attachInitialRotation;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType twoHandRotationType;
    public bool snapToSecondHand = true;
    private Quaternion initialRotationOffset;
    private XRBaseInteractor interactor;
    bool secondHandGrabbing;
    // Start is called before the first frame update
    void Start()
    {
        secondHandGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondHandGrabPoint.selectExited.AddListener(OnSecondHandRelease);
    }

    // Update is called once per frame
    void Update()
    {
        if (secondHandGrabbing == false && interactor)
        {
            try
            {
                interactor.attachTransform.rotation = interactor.transform.rotation;
            }
            catch { }
        }
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (secondInteractor && interactor)
        {
            if (rightHandGrabbing)
            {
                try
                {
                    secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.position = leftAttachSecond.position;
                    secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = leftAttachSecond.rotation;
                }
                catch 
                {
                    secondInteractor.GetComponent<RayInteractors>().handPhysics.transform.position = leftAttachSecond.position;
                    secondInteractor.GetComponent<RayInteractors>().handPhysics.transform.rotation = leftAttachSecond.rotation;
                }

            }
            if (leftHandGrabbing)
            {
                try
                {
                    secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.position = rightAttachSecond.position;
                    secondInteractor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = rightAttachSecond.rotation;
                }
                catch 
                {
                    secondInteractor.GetComponent<RayInteractors>().handPhysics.transform.position = rightAttachSecond.position;
                    secondInteractor.GetComponent<RayInteractors>().handPhysics.transform.rotation = rightAttachSecond.rotation;
                }

            }
            if (snapToSecondHand)
                try
                {
                    interactor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation();
                }
                catch
                {
                    interactor.GetComponent<RayInteractors>().handPhysics.transform.rotation = GetTwoHandRotation();
                }
            else
                try
                {
                    interactor.GetComponent<ControllerInteractors>().handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
                }
                catch
                {
                    interactor.GetComponent<RayInteractors>().handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
                }
        }
        base.ProcessInteractable(updatePhase);
    }
    public Quaternion GetTwoHandRotation()
    {
        Quaternion targetRotation;
        if (twoHandRotationType == TwoHandRotationType.None)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - interactor.attachTransform.position);
        }
        else if (twoHandRotationType == TwoHandRotationType.First)
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - interactor.attachTransform.position, interactor.transform.up);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(secondInteractor.attachTransform.position - interactor.attachTransform.position, secondInteractor.attachTransform.up);
        }
        return targetRotation;
    }
    public void OnSecondHandGrab(SelectEnterEventArgs args)
    {
        secondHandGrabbing = true;
        trackPosition = true;
        trackRotation = true;
        Debug.Log("SECOND HAND GRAB");
        try
        {
            secondInteractor = args.interactorObject.transform.GetComponent<ControllerInteractors>();
            secondInteractor.GetComponent<ControllerInteractors>().rayInteractor.maxRaycastDistance = 0;
        }
        catch
        {
            secondInteractor = args.interactorObject.transform.GetComponent<RayInteractors>();
            secondInteractor.GetComponent<RayInteractors>().maxRaycastDistance = 0;
        }
        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.attachTransform.rotation;
    }
    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        secondHandGrabbing = false;
        trackPosition = false;
        trackRotation = false;
        try
        {
            secondInteractor.GetComponent<ControllerInteractors>().rayInteractor.maxRaycastDistance = 30;
        }
        catch
        {
            secondInteractor.GetComponent<RayInteractors>().maxRaycastDistance = 30;
        }
        StartCoroutine(Delay());
        Debug.Log("SECOND HAND RELEASE");
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = selectingInteractor;
        Debug.Log("FIRST HAND GRAB");
        base.OnSelectEntered(args);
        try
        {
            attachInitialRotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation;
        }
        catch
        {
            attachInitialRotation = args.interactorObject.transform.GetComponent<RayInteractors>().attachTransform.localRotation;
            args.interactorObject.transform.GetComponent<RayInteractors>().maxRaycastDistance = 0;
        }
        StartCoroutine(DelaySecondGrab());
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        secondHandGrabPointCollider.enabled = false;
        Debug.Log("FIRST HAND RELEASE");
        base.OnSelectExited(args);
        if (rightHandGrabbing || leftHandGrabbing)
        {
            secondHandGrabPoint.enabled = false;
            secondHandGrabPoint.enabled = true;
            TwoHandInteractable interactable = GetComponent<TwoHandInteractable>();
            XRInteractionManager XRInteractionManager = interactionManager;
            interactor = secondInteractor;
            secondInteractor = interactor;
            try
            {
                if (rightHandGrabbing)
                {
                    GetComponent<TwoHandInteractable>().attachTransform = leftAttach;
                    GetComponent<TwoHandInteractable>().attachTransform = leftAttach;
                }
                else
                {
                    GetComponent<TwoHandInteractable>().attachTransform = rightAttach;
                    GetComponent<TwoHandInteractable>().attachTransform = rightAttach;
                }
                XRInteractionManager.SelectEnter(interactor, interactable);
                try
                {
                    attachInitialRotation = args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation;
                }
                catch
                {
                    attachInitialRotation = args.interactorObject.transform.GetComponent<RayInteractors>().attachTransform.localRotation;
                    args.interactorObject.transform.GetComponent<RayInteractors>().maxRaycastDistance = 0;
                }
            }
            catch { }
        }
        try
        {
            args.interactorObject.transform.GetComponent<ControllerInteractors>().attachTransform.localRotation = attachInitialRotation;
        }
        catch
        {
            args.interactorObject.transform.GetComponent<RayInteractors>().attachTransform.localRotation = attachInitialRotation;
            args.interactorObject.transform.GetComponent<RayInteractors>().maxRaycastDistance = 30;
        }
    }
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
        return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
    }
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.05f);

        if (!rightHandGrabbing || !leftHandGrabbing)
        {
            secondInteractor = null;
        }
    }
    public IEnumerator DelaySecondGrab()
    {
        secondHandGrabPointCollider.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        secondHandGrabPointCollider.gameObject.SetActive(true);
        secondHandGrabPointCollider.enabled = true;
    }
}
