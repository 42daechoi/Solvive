using UnityEngine;
using Photon.Pun;

public class FlashLightControl : MonoBehaviourPun
{
    [SerializeField] private GameObject whiteLight;
    private string firstPersonLightName = "WhiteLight_First";
    private GameObject firstPersonLight;

    public void ToggleFlashlight()
    {
        photonView.RPC("SyncToggleFlashlight", RpcTarget.All);
    }

    [PunRPC]
    private void SyncToggleFlashlight()
    {
        bool isActive = whiteLight.activeSelf;
        whiteLight.SetActive(!isActive);

        if (photonView.IsMine)
        {
            if (firstPersonLight == null)
            {
                firstPersonLight = GameObject.Find(firstPersonLightName);
            }

            if (firstPersonLight != null)
            {
                firstPersonLight.SetActive(!isActive);
            }
        }
    }
}
