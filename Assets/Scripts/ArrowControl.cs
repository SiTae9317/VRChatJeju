
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ArrowControl : UdonSharpBehaviour
{
    private bool isFire = false;
    private bool isGround = false;
    private float destroyTimer = 0.0f;
    private Vector3 beforePosition = Vector3.zero;
    public Transform lookatObj;

    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFire)
        {
            Vector3 curPos = gameObject.transform.position;
            Vector3 curVec = (curPos - beforePosition).normalized;
            if (curVec != Vector3.zero)
            {
                gameObject.transform.forward = curVec;
            }
            beforePosition = curPos;
        }
        else
        {
            if(lookatObj != null)
            {
                gameObject.transform.LookAt(lookatObj.transform);
            }
        }

        if(destroyTimer != 0.0f)
        {
            destroyTimer -= Time.deltaTime;

            if(destroyTimer < 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void fireArrow(float velocity)
    {
        Vector3 arrowVec = gameObject.transform.forward;
        Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.angularVelocity = arrowVec * velocity;
        rigid.velocity = arrowVec.normalized * velocity;
        isFire = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter end");
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!other.transform.name.ToLower().Contains("bow"))
        {
            Debug.Log("end");
            isFire = false;
            Destroy(gameObject.GetComponent<Rigidbody>());
            lookatObj = null;
            destroyTimer = 10.0f;
        }
    }
}
