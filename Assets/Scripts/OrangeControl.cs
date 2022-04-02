
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

    public void OnUnlock()
    {
        isInit = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
