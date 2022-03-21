
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportManager : UdonSharpBehaviour
{
    public Transform teleportPoint;
    void Start()
    {

    }

    public void OnPlayerTriggerEnter(VRC.SDKBase.VRCPlayerApi player)
    {
        player.TeleportTo(teleportPoint.position, teleportPoint.rotation);
    }
}
