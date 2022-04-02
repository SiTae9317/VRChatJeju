
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectControlTest : UdonSharpBehaviour
{
    //MeshRenderer mr;
    //private bool isTrigger;
    //public Material onMat;
    //public Material offMat;
    public OrangeControl oc;

    void Start()
    {
        //mr = gameObject.GetComponent<MeshRenderer>();
        //materialAction(isTrigger);
    }

    public void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnUnlock");
    }

    public void OnUnlock()
    {
        oc.OnUnlock();
        Destroy(gameObject);
        //isTrigger = !isTrigger;
        //materialAction(isTrigger);
    }

    //void materialAction(bool curStatus)
    //{
    //    if (curStatus)
    //    {
    //        mr.sharedMaterial = onMat;
    //    }
    //    else
    //    {
    //        mr.sharedMaterial = offMat;
    //    }
    //}
}
