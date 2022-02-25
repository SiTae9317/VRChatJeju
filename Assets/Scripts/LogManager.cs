
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class LogManager : UdonSharpBehaviour
{
    public Text debugText;
    public bool isEvent = false;
    public string logStr = "";
    private string[] logStrings = null;
    private int lineIndex = 0;

    void Start()
    {
        logStrings = new string[10];
        for (int i = 0; i < logStrings.Length; i++)
        {
            logStrings[i] = "";
        }
    }

    private void Update()
    {
        if (debugText != null && logStrings != null)
        {
            if (isEvent)
            {
                logStr = "";
                int curIndex = lineIndex;

                for (int i = 0; i < logStrings.Length; i++)
                {
                    logStr += logStrings[curIndex];
                    logStr += "\r\n";
                    curIndex++;
                    if (curIndex == logStrings.Length)
                    {
                        curIndex = 0;
                    }
                }
            }

            debugText.text = logStr;
        }
    }

    private void lineIndexUp()
    {
        lineIndex++;
        if (lineIndex == logStrings.Length)
        {
            lineIndex = 0;
        }
    }

    void setString(string inputData)
    {
        if (logStrings != null)
        {
            logStrings[lineIndex] = inputData;
            lineIndexUp();
        }
    }


    private void OnDrop()
    {
        setString("OnDrop");
    }

    private void OnOwnershipTransferred(VRC.SDKBase.VRCPlayerApi player)
    {
        setString("OnOwnershipTransferred");
    }

    private void OnPickup()
    {
        setString("OnPickup");
    }

    private void OnPickupUseDown()
    {
        setString("OnPickupUseDown");
    }

    private void OnPickupUseUp()
    {
        setString("OnPickupUseUp");
    }

    public void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputJump");
    }

    public void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputUse");
    }

    public void InputGrab(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputGrab");
    }

    public void InputDrop(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputDrop");
    }

    public void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputMoveHorizontal");
    }

    public void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputMoveVertical");
    }

    public void InputLookHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputLookHorizontal");
    }

    public void InputLookVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        setString("InputLookVertical");
    }
}
