
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class BoxTriggerTest : UdonSharpBehaviour
{
    public Text logTex;
    MeshRenderer mr;
    private bool isIn = false;
    public GameObject tarObject;
    public Material onMat;
    public Material offMat;

    void Start()
    {
        mr = tarObject.GetComponent<MeshRenderer>();
        materialAction(isIn);
    }
    public void OnPlayerTriggerEnter(VRC.SDKBase.VRCPlayerApi player)
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "EnterEvent");
    }
    public void OnPlayerTriggerExit(VRC.SDKBase.VRCPlayerApi player)
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ExitEvent");
    }

    public void EnterEvent()
    {
        logTex.text = "enter";
        bool curStatus = isIn;
        if (!curStatus)
        {
            isIn = !isIn;
            materialAction(isIn);
        }
    }

    public void ExitEvent()
    {
        logTex.text = "exit";
        bool curStatus = isIn;
        if (curStatus)
        {
            isIn = !isIn;
            materialAction(isIn);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerExit(Collider other)
    {
    }

    void materialAction(bool curStatus)
    {
        if (curStatus)
        {
            mr.sharedMaterial = onMat;
        }
        else
        {
            mr.sharedMaterial = offMat;
        }
    }
}
