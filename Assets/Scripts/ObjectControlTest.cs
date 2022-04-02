
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ObjectControlTest : UdonSharpBehaviour
{
    public OrangeControl oc;

    void Start()
    {
    }

    public void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnUnlock");
    }

    public void OnUnlock()
    {
        oc.OnUnlock();
        Destroy(gameObject);
    }
}
