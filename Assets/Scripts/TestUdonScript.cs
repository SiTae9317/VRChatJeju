using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TestUdonScript : UdonSharpBehaviour
{
    void Start()
    {
        
    }

    private void Update()
    {
        gameObject.transform.eulerAngles += Vector3.up * 10.0f * Time.deltaTime;
    }
}
