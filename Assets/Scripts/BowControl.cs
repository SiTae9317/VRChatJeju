using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class BowControl : UdonSharpBehaviour
{
    public LogManager lm;
    public VRC_Pickup currentPickup;

    public GameObject arrowPrefab;
    public GameObject wirePointObj;
    public GameObject wireBaseObj;

    public Transform leftPoint;
    public Transform rightPoint;

    public float bowPow = 30.0f;
    public float resetTime = 10.0f;

    public string pickupMessage = "";
    public string pullingMessage = "";

    private GameObject insArrow;

    private Vector3 wireOriPoint;
    private Vector3 beforePosition;

    private bool isPickupStatus = false;
    private int bowHandType = 0;
    private int arrowHandType = 0;

    private bool minimumPoint = false;
    private bool arrowDrag = false;
    private float saveTime = 0.0f;
    private float resetSaveTime = 0.0f;

    private bool leftDown = false;
    private bool rightDown = false;

    private bool shotHaptic = false;
    private bool setReset = false;

    private Vector3 resetPosition;
    private Quaternion resetRotation;

    private Vector3 localHandPos;
    private VRCPlayerApi curlocalPlayer = null;
    private VRCPlayerApi curTrackingPlayer = null;

    bool isTrackingMode = false;

    void Start()
    {
        resetPosition = gameObject.transform.position;
        resetRotation = gameObject.transform.rotation;
    }

    private void Update()
    {
        if(currentPickup.currentPlayer != null)
        {
            curTrackingPlayer = currentPickup.currentPlayer;
        }
        else
        {
            curTrackingPlayer = null;
        }

        if (isPickupStatus)
        {
            Vector3 handPos = getHumanBoneIndex();

            Vector3 wirePoint = wirePointObj.transform.position;

            float pointDis = Vector3.Distance(wirePoint, handPos);

            if (arrowDrag)
            {
                localHandPos = handPos;

                saveTime += Time.deltaTime;

                if (saveTime > 0.1f)
                {
                    saveTime = 0.0f;

                    Vector3 curHandNormal = wireBaseObj.transform.position - handPos;

                    float duration = Vector3.Distance(beforePosition, curHandNormal);

                    if (duration > 0.01f)
                    {
                        if (arrowHandType == 1)
                        {
                            Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, duration, 1.00f, pointDis);
                        }
                        else if (arrowHandType == 2)
                        {
                            Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, duration, 1.00f, pointDis);
                        }

                        beforePosition = curHandNormal;
                    }
                }
            }
            else
            {
                if (pointDis < 0.1f)
                {
                    if (!minimumPoint)
                    {
                        if (arrowHandType == 1)
                        {
                            Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, 0.1f, 1.00f, 1.00f);
                        }
                        else if (arrowHandType == 2)
                        {
                            Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, 0.1f, 1.00f, 1.00f);
                        }
                    }

                    lm.logStr = pullingMessage;

                    minimumPoint = true;
                }
                else
                {
                    lm.logStr = pickupMessage;

                    minimumPoint = false;
                }
            }

            if (shotHaptic)
            {
                shotHaptic = false;

                Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, 0.1f, 2.00f, 2.00f);
                Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, 0.1f, 2.00f, 2.00f);
            }
        }
        else
        {
            if(setReset)
            {
                resetSaveTime += Time.deltaTime;
                if(resetSaveTime > resetTime)
                {
                    resetSaveTime = 0.0f;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnResetTransform");
                }
            }
        }

        if(isTrackingMode)
        {
            if(arrowHandType == 1)
            {
                if (curTrackingPlayer != null)
                {
                    insArrow.transform.position = wirePointObj.transform.position = curTrackingPlayer.GetBonePosition(HumanBodyBones.LeftIndexProximal);
                }
            }
            else if(arrowHandType == 2)
            {
                if (curTrackingPlayer != null)
                {
                    insArrow.transform.position = wirePointObj.transform.position = curTrackingPlayer.GetBonePosition(HumanBodyBones.RightIndexProximal);
                }
            }
        }
    }

    private void settingStatus(bool flag, int type)
    {
        isPickupStatus = flag;
        arrowHandType = bowHandType = type;

        if (arrowHandType == 0)
        {
            arrowHandType = 0;
        }
        else if (arrowHandType == 1)
        {
            arrowHandType = 2;
        }
        else if (arrowHandType == 2)
        {
            arrowHandType = 1;
        }
    }

    Vector3 getHumanBoneIndex()
    {
        Vector3 handPos = Vector3.zero;


        if (arrowHandType == 1)
        {
            handPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftIndexProximal);
        }
        else if (arrowHandType == 2)
        {
            handPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightIndexProximal);
        }

        return handPos;
    }

    private void OnPickup()
    {
        curlocalPlayer = currentPickup.currentPlayer;
        curlocalPlayer.EnablePickups(false);

        settingStatus(true, (int)currentPickup.currentHand);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnResetSetting");
    }

    private void OnDrop()
    {
        if (curlocalPlayer != null)
        {
            curlocalPlayer.EnablePickups(true);
            curlocalPlayer = null;
        }

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnReleaseAction");

        settingStatus(false, 0);
    }

    public void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (args.handType == VRC.Udon.Common.HandType.LEFT)
        {
            leftDown = !leftDown;
        }
        else if (args.handType == VRC.Udon.Common.HandType.RIGHT)
        {
            rightDown = !rightDown;
        }

        if (arrowHandType == 1)
        {
            if (leftDown)
            {
                if (minimumPoint)
                {
                    if (arrowHandType == 1)
                    {
                        if(insArrow == null)
                        {
                            arrowDrag = true;
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnArrowLeftInstance");
                            beforePosition = wireBaseObj.transform.position - getHumanBoneIndex();
                            saveTime = 0.0f;
                        }
                    }
                }
            }
            else
            {
                if (arrowDrag)
                {
                    arrowDrag = false;
                    saveTime = 0.0f;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnArrowFire");
                    shotHaptic = true;
                }
            }
        }
        else if (arrowHandType == 2)
        {
            if (rightDown)
            {
                if (minimumPoint)
                {
                    if (arrowHandType == 2)
                    {
                        if (insArrow == null)
                        {
                            arrowDrag = true;
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnArrowRightInstance");
                            beforePosition = wireBaseObj.transform.position - getHumanBoneIndex();
                            saveTime = 0.0f;
                        }
                    }
                }
            }
            else
            {
                if (arrowDrag)
                {
                    arrowDrag = false;
                    saveTime = 0.0f;
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnArrowFire");
                    shotHaptic = true;
                }
            }
        }
    }

    public void OnArrowLeftInstance()
    {
        if (curTrackingPlayer != null)
        {
            Vector3 leftPointPosition = curTrackingPlayer.GetBonePosition(HumanBodyBones.LeftIndexProximal);
            insArrow = VRCInstantiate(arrowPrefab);
            insArrow.GetComponent<ArrowControl>().lookatObj = leftPoint;
            insArrow.transform.position = leftPointPosition;
            isTrackingMode = true;
            arrowHandType = 1; 
        }
    }

    public void OnArrowRightInstance()
    {
        if (curTrackingPlayer != null)
        {
            Vector3 rightPointPosition = curTrackingPlayer.GetBonePosition(HumanBodyBones.RightIndexProximal);
            insArrow = VRCInstantiate(arrowPrefab);
            insArrow.GetComponent<ArrowControl>().lookatObj = rightPoint;
            insArrow.transform.position = rightPointPosition;
            isTrackingMode = true;
            arrowHandType = 2;
        }
    }

    public void OnArrowFire()
    {
        isTrackingMode = false;
        Vector3 disVec1 = wirePointObj.transform.position;
        wirePointObj.transform.localPosition = wireBaseObj.transform.localPosition;
        Vector3 disVec2 = wirePointObj.transform.position;

        if(insArrow != null)
        {
            insArrow.GetComponent<ArrowControl>().fireArrow(Vector3.Distance(disVec1, disVec2) * bowPow);
        }
        insArrow = null;
    }

    public void OnReleaseAction()
    {
        isTrackingMode = false;
        isPickupStatus = false;
        bowHandType = 0;
        arrowHandType = 0;

        minimumPoint = false;
        arrowDrag = false;
        saveTime = 0.0f;
        shotHaptic = false;

        Vector3 disVec1 = wirePointObj.transform.position;
        wirePointObj.transform.localPosition = wireBaseObj.transform.localPosition;
        Vector3 disVec2 = wirePointObj.transform.position;

        if (insArrow != null)
        {
            Destroy(insArrow);
            insArrow = null;
        }
    }

    public void OnResetSetting()
    {
        resetSaveTime = 0.0f;
        setReset = isPickupStatus;
    }

    public void OnResetTransform()
    {
        if (setReset)
        {
            setReset = false;

            gameObject.transform.position = resetPosition;
            gameObject.transform.rotation = resetRotation;
        }
    }
}