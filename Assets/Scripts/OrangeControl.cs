
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrangeControl : UdonSharpBehaviour
{
    public float resetTime = 10.0f;
    private Vector3 keepPosition;
    private Quaternion keepRotation;
    private bool setReset = false;
    private float saveResetTime = 0.0f;
    private Rigidbody rigid;

    void Start()
    {
        keepPosition = gameObject.transform.position;
        keepRotation = gameObject.transform.rotation;
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(setReset)
        {
            saveResetTime += Time.deltaTime;
            if(saveResetTime > resetTime)
            {
                saveResetTime = 0.0f;
                setReset = false;

                gameObject.transform.position = keepPosition;
                gameObject.transform.rotation = keepRotation;

                if(rigid.useGravity)
                {
                    rigid.useGravity = false;
                }
            }
        }
    }

    private void OnPickup()
    {
        setReset = false;
        saveResetTime = 0.0f;
        if(!rigid.useGravity)
        {
            rigid.useGravity = true;
        }
        //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnGravityUse");
    }

    private void OnDrop()
    {
        setReset = true;
    }

    public void OnGravityUse()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }
}
