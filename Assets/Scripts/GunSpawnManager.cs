
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GunSpawnManager : UdonSharpBehaviour
{
    public GameObject gunPrefab;

    void Start()
    {
    }

    public void OnPlayerJoined(VRC.SDKBase.VRCPlayerApi player)
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnUserJoinGenGun");
    }

    public void OnUserJoinGenGun()
    {
        GameObject newGun = VRCInstantiate(gunPrefab);
        newGun.transform.position = Vector3.up * 1.13f;
        newGun.transform.rotation = Quaternion.identity;
        newGun.transform.localScale = Vector3.one * 0.2947223f;
    }
}
