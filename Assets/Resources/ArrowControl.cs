using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour
{
    private bool isFire = false;
    private bool isGround = false;
    private Vector3 beforePosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        

        Vector2[] uvData = mesh.uv;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        Vector4 lightmapData = mr.lightmapScaleOffset;
        
        Debug.Log(mr.lightmapIndex);

        Debug.Log(lightmapData.x + " " + lightmapData.y + " " + lightmapData.z + " " + lightmapData.w);

        Debug.Log("uv ============================");
        testLog(mesh.uv);

        Debug.Log("uv2 ============================");
        testLog(mesh.uv2);

        Debug.Log("uv3 ============================");
        testLog(mesh.uv3);

        Debug.Log("uv4 ============================");
        testLog(mesh.uv4);

        Debug.Log("uv5 ============================");
        testLog(mesh.uv5);

    }

    void testLog(Vector2[] uvData)
    {
        string logData = "";
        if (uvData != null)
        {
            for (int i = 0; i < uvData.Length; i++)
            {
                logData += uvData[i].x + " " + uvData[i].y;
            }

            Debug.Log(logData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curPos = gameObject.transform.position;

        if (isFire)
        {
            Vector3 curVec = (curPos - beforePosition).normalized;
            if(curVec != Vector3.zero)
            {
                gameObject.transform.up = curVec;
            }
        }
        beforePosition = curPos;
    }

    public void fireArrow(float velocity)
    {
        Vector3 arrowVec = gameObject.transform.up;
        gameObject.GetComponent<Rigidbody>().angularVelocity = arrowVec * velocity;
        gameObject.GetComponent<Rigidbody>().velocity = arrowVec.normalized * velocity;
        isFire = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter end");
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("end");
        isFire = false;
        Destroy(gameObject.GetComponent<Rigidbody>());
    }
}
