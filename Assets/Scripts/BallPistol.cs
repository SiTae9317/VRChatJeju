
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BallPistol : UdonSharpBehaviour
{
    private string curName = "htkwak ";
    private Light flashLight;
    public GameObject ammo;
    public Transform spawnPoint;
    private VRCPlayerApi localPlayer = null;
    public VRC_Pickup currentVRCPickup;
    private bool isCheck= false;
    private float saveTime = 0.0f;
    private MeshFilter mf;
    private Mesh mesh;
    private Vector3[] vertices = null;
    private Vector3[] normals = null;
    private int[] triangles = null;
    private Vector2[] uv = null;
    private float adderVal = 0.0f;
    private bool inInnder = false;


    private void Update()
    {
        if (isCheck)
        {
            saveTime += Time.deltaTime;

            if (saveTime > 0.1f)
            {
                Debug.Log(curName + " testCor");
                saveTime = 0.0f;
                Networking.LocalPlayer.PlayHapticEventInHand(currentVRCPickup.currentHand, 0.05f, 0.05f, 0.05f);
            }
        }

        if(!inInnder)
        {
            if(adderVal < 5.0f)
            {
                //Debug.Log(curName + " increase");
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += normals[i];
                }
                adderVal += 1.0f;
            }
            else
            {
                adderVal = 0.0f;
                inInnder = !inInnder;
            }
        }
        else
        {
            //Debug.Log(curName + " decrease");
            if (adderVal < 5.0f)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] -= normals[i];
                }
                adderVal += 1.0f;
            }
            else
            {
                adderVal = 0.0f;
                inInnder = !inInnder;
            }
        }

        mesh.vertices = vertices;

        mf.mesh = mesh;
    }

    void Start()
    {
        curName += gameObject.name;
        flashLight = GetComponentInChildren<Light>();
        mf = GetComponentInChildren<MeshFilter>();
        mesh = mf.mesh;

        int verticesCount = mesh.vertexCount;
        vertices = new Vector3[verticesCount];
        normals = mesh.normals;
        triangles = mesh.triangles;
        uv = mesh.uv;

        Vector3[] curVec = mesh.vertices;

        for(int i = 0; i < verticesCount; i++)
        {
            vertices[i] = curVec[i];
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.normals = normals;
        newMesh.triangles = triangles;
        newMesh.uv = uv;

        mesh = newMesh;
    }

    private void OnDrop()
    {
        Debug.Log(curName + " OnDrop");
        
    }

    private void OnOwnershipTransferred(VRC.SDKBase.VRCPlayerApi player)
    {
        Debug.Log(curName + " OnOwnershipTransferred");
        //player.GetBonePosition(HumanBodyBones.RightHand)
        //localPlayer = player;
        //player.GetPickupInHand(VRC_Pickup.PickupHand.Left).
        //player.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, 1.0f, 1.0f, 1.0f);
    }

    private void OnPickup()
    {
        Debug.Log(curName + " OnPickup");
        flashLight.enabled = false;
    }

    private void OnPickupUseDown()
    {
        Debug.Log(curName + " OnPickupUseDown");
        //if(localPlayer != null)
        //{
        //    Networking.LocalPlayer.PlayHapticEventInHand
        //    //localPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, 1.0f, 1.0f, 1.0f);
        //}

        if(!isCheck)
        {
            isCheck = true;
            //StartCoroutine(testCor());
        }

        Networking.LocalPlayer.PlayHapticEventInHand(currentVRCPickup.currentHand, 0.1f, 1.0f, 1.0f);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnFireEvent");
        //gameObject.GetComponent<VRC_Pickup>()?.PlayHaptics();
    }

    public void OnFireEvent()
    {
        Debug.Log(curName + " OnFireEvent");
        GameObject ball = VRCInstantiate(ammo);
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        ball.transform.position = spawnPoint.position;
        ball.transform.rotation = spawnPoint.rotation;

        rb.velocity = ball.transform.forward * 20.0f;
    }

    private void OnPickupUseUp()
    {
        isCheck = false;
        Debug.Log(curName + " OnPickupUseUp");
    }

    public void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputUse(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputGrab(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputDrop(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputMoveHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputMoveVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputLookHorizontal(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    public void InputLookVertical(float value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        ;
    }

    //public IEnumerator testCor()
    //{
    //    while(isCheck)
    //    {
    //        saveTime += Time.deltaTime; 

    //        if(saveTime > 0.1f)
    //        {
    //            Debug.Log(curName + " testCor");
    //            saveTime = 0.0f;
    //            Networking.LocalPlayer.PlayHapticEventInHand(currentVRCPickup.currentHand, 0.15f, 0.25f, 0.25f);
    //        }

    //        yield return null;
    //    }
    //}
}
