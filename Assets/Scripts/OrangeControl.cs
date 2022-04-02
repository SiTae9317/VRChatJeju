
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrangeControl : UdonSharpBehaviour
{
    public GameObject parentPrefab;

    void Start()
    {
        
    }

    private void OnPickup()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnGravityUse");
    }

    private void OnDrop()
    {
    }

    public void OnGravityUse()
    {
        GameObject newGo = VRCInstantiate(parentPrefab);

        newGo.transform.position = gameObject.transform.position;
        newGo.transform.rotation = gameObject.transform.rotation;
        newGo.transform.localScale = gameObject.transform.localScale;

        Destroy(gameObject);
    }
}
