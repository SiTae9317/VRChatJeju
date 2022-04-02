using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class BowControl : UdonSharpBehaviour
{
    public LogManager lm;
    public VRC_Pickup currentPickup;
    //public GameObject handPointObj;

    public GameObject arrowPrefab;
    public GameObject wirePointObj;
    public GameObject wireBaseObj;

    public Transform leftPoint;
    public Transform rightPoint;

    public float bowPow = 30.0f;

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

    private bool leftDown = false;
    private bool rightDown = false;

    private bool shotHaptic = false;

    private Vector3 localHandPos;
    private int currentBowIndex = -1;
    VRCPlayerApi curlocalPlayer;
    float playTime = 0.0f;

    void Start()
    {
        currentBowIndex = int.Parse(gameObject.name.Split(' ')[1]);
        //wireOriPoint = wirePointObj.transform.localPosition;
    }

    private void Update()
    {
        playTime += Time.deltaTime;
        lm.logStr = playTime.ToString();

        if (currentPickup.currentPlayer == null)
        {
            lm.logStr += "\r\ncp null";
        }
        else
        {
            lm.logStr += "\r\ncp has " + currentPickup.currentPlayer.IsValid().ToString();
        }

        if (isPickupStatus)
        {
            Vector3 handPos = getHumanBoneIndex();

            //lm.logStr = handPos.x.ToString() + " " + handPos.y.ToString() + " " + handPos.z.ToString();
            //handPointObj.transform.position = handPos;

            Vector3 wirePoint = wirePointObj.transform.position;

            float pointDis = Vector3.Distance(wirePoint, handPos);

            if (arrowDrag)
            {
                localHandPos = handPos;

                if (arrowHandType == 1)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnWirePositionLeftHand");
                }
                else if (arrowHandType == 2)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnWirePositionRightHand");
                }
                //wirePointObj.transform.position = handPos;

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
                    //if (pickupMessage == "")
                    //{
                    //    lm.logStr = "Use button";
                    //}
                    //else
                    //{
                    //lm.logStr = pullingMessage;
                    //}
                    minimumPoint = true;
                }
                else
                {
                    //if(pickupMessage == "")
                    //{
                    //    lm.logStr = "Hold the bowstring\nwith the use button";
                    //}
                    //else
                    //{
                    //lm.logStr = pickupMessage;
                    //}
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
    }

    public void OnWirePositionLeftHand()
    {
        if (currentPickup.currentPlayer.IsValid())
        {
            insArrow.transform.position = wirePointObj.transform.position = currentPickup.currentPlayer.GetBonePosition(HumanBodyBones.LeftIndexProximal);
        }
    }

    public void OnWirePositionRightHand()
    {
        if (currentPickup.currentPlayer.IsValid())
        {
            insArrow.transform.position = wirePointObj.transform.position = currentPickup.currentPlayer.GetBonePosition(HumanBodyBones.RightIndexProximal);
        }
    }

    //public void OnWirePositionRelease()
    //{
    //    wirePointObj.transform.localPosition = wireOriPoint;
    //}

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

    private void OnDrop()
    {
        if (curlocalPlayer != null)
        {
            curlocalPlayer.EnablePickups(true);
            curlocalPlayer = null;
        }

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnReleaseAction" + currentBowIndex.ToString());

        settingStatus(false, 0);
    }

    private void OnOwnershipTransferred(VRC.SDKBase.VRCPlayerApi player)
    {
        ;
    }

    private void OnPickup()
    {
        curlocalPlayer = currentPickup.currentPlayer;
        curlocalPlayer.EnablePickups(false);

        settingStatus(true, (int)currentPickup.currentHand);
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

    Vector3 getHumanAnotherBoneIndex()
    {
        Vector3 handPos = Vector3.zero;

        if (arrowHandType == 2)
        {
            handPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftIndexProximal);
        }
        else if (arrowHandType == 1)
        {
            handPos = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightIndexProximal);
        }

        return handPos;
    }

    private void OnPickupUseDown()
    {
        ;
    }

    private void OnPickupUseUp()
    {
        ;
    }

    public void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        //if (minimumPoint)
        //{
        //    if (args.handType == VRC.Udon.Common.HandType.LEFT)
        //    {
        //        if(arrowHandType == 1)
        //        {
        //            arrowDrag = !arrowDrag;
        //            if(arrowDrag)
        //            {
        //                beforePosition = getHumanBoneIndex();
        //            }
        //            saveTime = 0.0f;
        //        }
        //    }
        //    else if (args.handType == VRC.Udon.Common.HandType.RIGHT)
        //    {
        //        if(arrowHandType == 2)
        //        {
        //            arrowDrag = !arrowDrag;
        //            if (arrowDrag)
        //            {
        //                beforePosition = getHumanBoneIndex();
        //            }
        //            saveTime = 0.0f;
        //        }
        //    }
        //}

        if(isPickupStatus)
        {
            if (args.handType == VRC.Udon.Common.HandType.LEFT)
            {
                leftDown = !leftDown;

                if (leftDown)
                {
                    if (minimumPoint)
                    {
                        if (arrowHandType == 1)
                        {
                            arrowDrag = true;
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnArrowLeftInstance");
                            beforePosition = wireBaseObj.transform.position - getHumanBoneIndex();
                            saveTime = 0.0f;
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
                        //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnWirePositionRelease");
                        shotHaptic = true;
                    }
                }
            }
            else if (args.handType == VRC.Udon.Common.HandType.RIGHT)
            {
                rightDown = !rightDown;

                if (rightDown)
                {
                    if (minimumPoint)
                    {
                        if (arrowHandType == 2)
                        {
                            arrowDrag = true;
                            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnArrowRightInstance");
                            beforePosition = wireBaseObj.transform.position - getHumanBoneIndex();
                            saveTime = 0.0f;
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
                        //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnWirePositionRelease");
                        shotHaptic = true;
                    }
                }
            }
        }
    }

    public void OnArrowLeftInstance()
    {
        insArrow = VRCInstantiate(arrowPrefab);
        insArrow.GetComponent<ArrowControl>().lookatObj = leftPoint;//currentPickup.ExactGun;
        insArrow.transform.position = currentPickup.currentPlayer.GetBonePosition(HumanBodyBones.LeftIndexProximal);
    }

    public void OnArrowRightInstance()
    {
        insArrow = VRCInstantiate(arrowPrefab);
        insArrow.GetComponent<ArrowControl>().lookatObj = rightPoint;// currentPickup.ExactGun;
        insArrow.transform.position = currentPickup.currentPlayer.GetBonePosition(HumanBodyBones.RightIndexProximal);
    }

    public void OnArrowFire()
    {
        Vector3 disVec1 = wirePointObj.transform.position;
        wirePointObj.transform.localPosition = wireBaseObj.transform.localPosition;
        Vector3 disVec2 = wirePointObj.transform.position;

        insArrow.GetComponent<ArrowControl>().fireArrow(Vector3.Distance(disVec1, disVec2) * bowPow);
        insArrow = null;
    }

    public void OnReleaseAction0()
    {
        if (currentBowIndex == 0)
        {
            bowReset();
        }
    }

    public void OnReleaseAction1()
    {
        if (currentBowIndex == 1)
        {
            bowReset();
        }
    }

    public void OnReleaseAction2()
    {
        if (currentBowIndex == 2)
        {
            bowReset();
        }
    }

    public void OnReleaseAction3()
    {
        if (currentBowIndex == 3)
        {
            bowReset();
        }
    }

    public void OnReleaseAction4()
    {
        if (currentBowIndex == 4)
        {
            bowReset();
        }
    }

    public void OnReleaseAction5()
    {
        if (currentBowIndex == 5)
        {
            bowReset();
        }
    }

    public void OnReleaseAction6()
    {
        if (currentBowIndex == 6)
        {
            bowReset();
        }
    }

    public void OnReleaseAction7()
    {
        if (currentBowIndex == 7)
        {
            bowReset();
        }
    }

    public void OnReleaseAction8()
    {
        if (currentBowIndex == 8)
        {
            bowReset();
        }
    }

    public void OnReleaseAction9()
    {
        if (currentBowIndex == 9)
        {
            bowReset();
        }
    }

    public void OnReleaseAction10()
    {
        if (currentBowIndex == 10)
        {
            bowReset();
        }
    }

    public void OnReleaseAction11()
    {
        if (currentBowIndex == 11)
        {
            bowReset();
        }
    }

    public void OnReleaseAction12()
    {
        if (currentBowIndex == 12)
        {
            bowReset();
        }
    }
    public void bowReset()
    {
        isPickupStatus = false;
        bowHandType = 0;
        arrowHandType = 0;
    
        minimumPoint = false;
        arrowDrag = false;
        saveTime = 0.0f;

        leftDown = false;
        rightDown = false;
        shotHaptic = false;

        Vector3 disVec1 = wirePointObj.transform.position;
        wirePointObj.transform.localPosition = wireBaseObj.transform.localPosition;
        Vector3 disVec2 = wirePointObj.transform.position;

        if (insArrow != null)
        {
            lm.logStr = "destroy arrow";
            Destroy(insArrow);
            //insArrow.GetComponent<ArrowControl>().fireArrow(Vector3.Distance(disVec1, disVec2) * bowPow);
            insArrow = null;
        }
        else
        {
            lm.logStr = "arrow null";
        }
    }
}