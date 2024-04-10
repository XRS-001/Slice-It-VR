using RootMotion;
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
    public XRBaseInteractor secondInteractor;
    private Quaternion attachInitialRotation;
    public enum TwoHandRotationType { None, First, Second };
    public TwoHandRotationType twoHandRotationType;
    public bool snapToSecondHand = true;
    private Quaternion initialRotationOffset;
    private XRBaseInteractor interactor;
    bool secondHandGrabbing;

    void Start()
    {
        secondHandGrabPoint.selectEntered.AddListener(OnSecondHandGrab);
        secondHandGrabPoint.selectExited.AddListener(OnSecondHandRelease);
    }

    void Update()
    {
        if (!secondHandGrabbing && interactor)
        {
            if (interactor.attachTransform != null)
            {
                interactor.attachTransform.rotation = interactor.transform.rotation;
            }
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (secondInteractor && interactor)
        {
            ControllerInteractors controllerCheck = interactor?.GetComponent<ControllerInteractors>();
            RayInteractors raycasterCheck = interactor?.GetComponent<RayInteractors>();
            if(controllerCheck)
            {
                interactor.GetComponent<ControllerInteractors>().secondHandGrabbing = true;

            }
            if (raycasterCheck)
            {
                interactor.GetComponent<RayInteractors>().secondHandGrabbing = true;
            }
            if (rightHandGrabbing)
            {
                ControllerInteractors controller = secondInteractor.GetComponent<ControllerInteractors>();
                RayInteractors raycaster = secondInteractor.GetComponent<RayInteractors>();

                if (controller != null)
                {
                    controller.handPhysics.transform.position = leftAttachSecond.position;
                    controller.handPhysics.transform.rotation = leftAttachSecond.rotation;
                }
                else if (raycaster != null)
                {
                    raycaster.handPhysics.transform.position = leftAttachSecond.position;
                    raycaster.handPhysics.transform.rotation = leftAttachSecond.rotation;
                }
            }
            if (leftHandGrabbing)
            {
                ControllerInteractors controller = secondInteractor.GetComponent<ControllerInteractors>();
                RayInteractors raycaster = secondInteractor.GetComponent<RayInteractors>();

                if (controller != null)
                {
                    controller.handPhysics.transform.position = rightAttachSecond.position;
                    controller.handPhysics.transform.rotation = rightAttachSecond.rotation;
                }
                else if (raycaster != null)
                {
                    raycaster.handPhysics.transform.position = rightAttachSecond.position;
                    raycaster.handPhysics.transform.rotation = rightAttachSecond.rotation;
                }
            }
            if (snapToSecondHand)
            {
                ControllerInteractors controller = interactor.GetComponent<ControllerInteractors>();
                RayInteractors raycaster = interactor.GetComponent<RayInteractors>();

                if (controller != null)
                {
                    if (controller.handPhysics != null)
                    {
                        controller.handPhysics.transform.rotation = GetTwoHandRotation();
                    }
                }
                else if (raycaster != null)
                {
                    if (raycaster.handPhysics != null)
                    {
                        raycaster.handPhysics.transform.rotation = GetTwoHandRotation();
                    }
                }
            }
            else
            {
                ControllerInteractors controller = interactor.GetComponent<ControllerInteractors>();
                RayInteractors raycaster = interactor.GetComponent<RayInteractors>();

                if (controller != null)
                {
                    if (controller.handPhysics != null)
                    {
                        controller.handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
                    }
                }
                else if (raycaster != null)
                {
                    if (raycaster.handPhysics != null)
                    {
                        raycaster.handPhysics.transform.rotation = GetTwoHandRotation() * initialRotationOffset;
                    }
                }
            }
        }
        else
        {
            ControllerInteractors controllerCheck = interactor?.GetComponent<ControllerInteractors>();
            RayInteractors raycasterCheck = interactor?.GetComponent<RayInteractors>();
            if (controllerCheck)
            {
                interactor.GetComponent<ControllerInteractors>().secondHandGrabbing = false;

            }
            if (raycasterCheck)
            {
                interactor.GetComponent<RayInteractors>().secondHandGrabbing = false;
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
        Debug.Log("SECOND HAND GRAB");
        // Attempt to get both ControllerInteractors and RayInteractors components
        ControllerInteractors controllerInteractor = args.interactorObject.transform.GetComponent<ControllerInteractors>() ?? null;

        if (controllerInteractor != null)
        {
            secondInteractor = controllerInteractor;
            controllerInteractor.rayInteractor.maxRaycastDistance = 0f;
        }
        else
        {
            RayInteractors rayInteractor = args.interactorObject.transform.GetComponent<RayInteractors>();
            rayInteractor.maxRaycastDistance = 0;
            secondInteractor = rayInteractor;
            if (rayInteractor.gameObject.CompareTag("RightHand"))
            {
                attachTransform = rightAttachSecond;
            }
            else
            {
                attachTransform = leftAttachSecond;
            }
        }
        initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * interactor.attachTransform.rotation;
    }

    public void OnSecondHandRelease(SelectExitEventArgs args)
    {
        secondHandGrabbing = false;

        if (secondInteractor != null)
        {
            RayInteractors raycaster = secondInteractor.GetComponent<RayInteractors>();
            if (raycaster != null)
            {
                raycaster.maxRaycastDistance = 30;
            }
        }

        StartCoroutine(Delay());
        Debug.Log("SECOND HAND RELEASE");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        secondHandGrabPoint.enabled = true;
        interactor = selectingInteractor;
        Debug.Log("FIRST HAND GRAB");
        base.OnSelectEntered(args);

        ControllerInteractors controller = args.interactorObject.transform.GetComponent<ControllerInteractors>();
        RayInteractors raycaster = args.interactorObject.transform.GetComponent<RayInteractors>();

        if (controller != null)
        {
            attachInitialRotation = controller.attachTransform.localRotation;
        }
        else if (raycaster != null)
        {
            attachInitialRotation = raycaster.attachTransform.localRotation;
            raycaster.maxRaycastDistance = 0;
        }

        StartCoroutine(DelaySecondGrab());
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("FIRST HAND RELEASE");
        base.OnSelectExited(args);
        secondHandGrabPoint.enabled = false;
        secondHandGrabPoint.enabled = true;
        secondHandGrabPoint.enabled = false;
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

