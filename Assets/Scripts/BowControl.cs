
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

    public GameObject wirePointObj;

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

    void Start()
    {
        wireOriPoint = wirePointObj.transform.localPosition;
    }

    private void Update()
    {
        if(isPickupStatus)
        {
            Vector3 handPos = getHumanBoneIndex();

            //lm.logStr = handPos.x.ToString() + " " + handPos.y.ToString() + " " + handPos.z.ToString();
            //handPointObj.transform.position = handPos;

            Vector3 wirePoint = wirePointObj.transform.position;

            float pointDis = Vector3.Distance(wirePoint, handPos);
            if (pointDis < 0.1f)
            {
                lm.logStr = "Grip";
                minimumPoint = true;
            }
            else
            {
                lm.logStr = "";
                minimumPoint = false;
            }

            if (arrowDrag)
            {
                wirePointObj.transform.position = handPos;

                saveTime += Time.deltaTime;

                if (saveTime > 0.1f)
                {
                    saveTime = 0.0f;

                    float duration = Vector3.Distance(beforePosition, handPos);

                    if (duration > 0.0f)
                    {
                        if (arrowHandType == 1)
                        {
                            Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, duration, 1.00f, pointDis);
                        }
                        else if (arrowHandType == 2)
                        {
                            Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, duration, 1.00f, pointDis);
                        }

                        beforePosition = handPos;
                    }
                }
            }
            else
            {
                wirePointObj.transform.localPosition = wireOriPoint;
            }

            if(shotHaptic)
            {
                shotHaptic = false;

                Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, 0.1f, 2.00f, 2.00f);
                Networking.LocalPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, 0.1f, 2.00f, 2.00f);
            }
        }
    }

    private void settingStatus(bool flag, int type)
    {
        isPickupStatus = flag;
        arrowHandType = bowHandType = type;

        if(arrowHandType == 0)
        {
            arrowHandType = 0;
        }
        else if(arrowHandType == 1)
        {
            arrowHandType = 2;
        }
        else if(arrowHandType == 2)
        {
            arrowHandType = 1;
        }
    }

    private void OnDrop()
    {
        settingStatus(false, 0);
    }

    private void OnOwnershipTransferred(VRC.SDKBase.VRCPlayerApi player)
    {
        ;
    }

    private void OnPickup()
    {
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

        if (args.handType == VRC.Udon.Common.HandType.LEFT)
        {
            leftDown = !leftDown;

            if(leftDown)
            {
                if(minimumPoint)
                {
                    if (arrowHandType == 1)
                    {
                        arrowDrag = true;
                        beforePosition = getHumanBoneIndex();
                        saveTime = 0.0f;
                    }
                }
            }
            else
            {
                if(arrowDrag)
                {
                    arrowDrag = false;
                    saveTime = 0.0f;
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
                        beforePosition = getHumanBoneIndex();
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
                    shotHaptic = true;
                }
            }
        }
    }

    public void InputGrab(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputDrop(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputLookHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputLookVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }
}
