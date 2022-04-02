
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrangeControl : UdonSharpBehaviour
{
    public GameObject parentPrefab;
    private bool isInit = false;

    private Vector3 keepPosition;
    private Quaternion keepRotation;

    void Start()
    {
        keepPosition = gameObject.transform.position;
        keepRotation = gameObject.transform.rotation;
    }

    private void Update()
    {
        if(!isInit)
        {
            gameObject.transform.position = keepPosition;
            gameObject.transform.rotation = keepRotation;
        }
    }

    //private void OnPickup()
    //{
    //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnGravityUse");
    //}

    //private void OnDrop()
    //{
    //}

    //public void OnGravityUse()
    //{
    //    GameObject newGo = VRCInstantiate(parentPrefab);

    //    newGo.transform.position = gameObject.transform.position;
    //    newGo.transform.rotation = gameObject.transform.rotation;
    //    newGo.transform.localScale = gameObject.transform.localScale;

    //    Destroy(gameObject);
    //}

    public void OnUnlock()
    {
        isInit = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
