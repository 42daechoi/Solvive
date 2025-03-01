using UnityEngine;
using Photon.Pun;

public class FlashLightControl : MonoBehaviourPun
{
    [SerializeField] private GameObject whiteLight;

    public void ToggleFlashlight()
    {
        photonView.RPC("SyncToggleFlashlight", RpcTarget.All);
    }

    [PunRPC]
    private void SyncToggleFlashlight()
    {
        bool isActive = whiteLight.activeSelf;
        whiteLight.SetActive(!isActive);
    }
}
