
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectControlTest : UdonSharpBehaviour
{
    MeshRenderer mr;
    private bool isTrigger;
    public Material onMat;
    public Material offMat;

    void Start()
    {
        mr = gameObject.GetComponent<MeshRenderer>();
        materialAction(isTrigger);
    }

    public void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "MyButtonClickEvent");
    }

    public void MyButtonClickEvent()
    {
        isTrigger = !isTrigger;
        materialAction(isTrigger);
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
