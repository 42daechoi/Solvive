using UnityEngine;
using Photon.Pun;
using UnityEditor;

public class FlashLightControl : MonoBehaviourPun
{
    [SerializeField] private GameObject whiteLight;
    private bool isOn = false;

    public void ToggleFlashlight()
    {
        isOn = !isOn;
        photonView.RPC("SyncToggleFlashlight", RpcTarget.All, isOn);
    }

    [PunRPC]
    private void SyncToggleFlashlight(bool isOn)
    {
        Light light = whiteLight.GetComponent<Light>();
        if (light != null)
        {
            if (photonView.transform.root.GetComponent<PhotonView>().IsMine)
            {
                light.enabled = false;
                EventManager_Game.Instance.InvokeFPSlightToggle(isOn);
            }
            else
            {
                light.enabled = isOn;
            }
        }
    }
}
