using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ObjectPool : MonoBehaviourPunCallbacks
{
    public static ObjectPool instance;

    private void Awake()
    {
        instance = this;
    }

    private Dictionary<int, GameObject> objectPool = new Dictionary<int, GameObject>();

    public GameObject GetObject(int viewID, Vector3 position, Quaternion rotation)
    {
        if (objectPool.ContainsKey(viewID))
        {
            GameObject obj = objectPool[viewID];

            photonView.RPC("SyncGetObject", RpcTarget.All, viewID, position, rotation);
            return obj;
        }
        Debug.Log($"ObjectPool : ViewID {viewID}를 가진 아이템이 없습니다.");
        return null;
    }

    [PunRPC]
    private void SyncGetObject(int viewID, Vector3 position, Quaternion rotation)
    {
        if (objectPool.ContainsKey(viewID))
        {
            GameObject obj = objectPool[viewID];
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
    }

    public void ReturnObject(GameObject obj)
    {
        int viewID = obj.GetPhotonView().ViewID;
        photonView.RPC("SyncReturnObject", RpcTarget.All, viewID);
    }

    [PunRPC]
    private void SyncReturnObject(int viewID)
    {
        if (!objectPool.ContainsKey(viewID))
        {
            objectPool[viewID] = PhotonView.Find(viewID).gameObject;
        }
        objectPool[viewID].SetActive(false);
    }
}
