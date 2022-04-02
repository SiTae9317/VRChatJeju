
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrangeControl : UdonSharpBehaviour
{
    void Start()
    {
        
    }

    private void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnGravityUse");
    }

    public void OnGravityUse()
    {
        Destroy(GetComponent<Rigidbody>());
        //gameObject.GetComponent<Rigidbody>().useGravity = true;
    }
}
