using System.Collections;
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

	private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

	public GameObject GetObject(string itemName, Vector3 position, Quaternion rotation)
	{
		if (pool.ContainsKey(itemName) && pool[itemName].Count > 0)
		{
			int viewID = pool[itemName].Peek().GetPhotonView().ViewID;
            GameObject obj = PhotonView.Find(viewID).gameObject;

            photonView.RPC("SyncGetObject", RpcTarget.All, itemName, viewID, position, rotation);

            return obj;
        }
		Debug.Log("ObjectPool : 풀에 해당 아이템이 없습니다.");
		return null;
    }

	[PunRPC]
	private void SyncGetObject(string itemName, int viewID, Vector3 position, Quaternion rotation)
	{
		if (pool.ContainsKey(itemName) && pool[itemName].Count > 0)
		{
			GameObject obj = PhotonView.Find(viewID).gameObject;
	
			if (pool[itemName].Peek() == obj)
			{
				pool[itemName].Dequeue();
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				obj.SetActive(true);
			}
		}
	}

	public void ReturnObject(GameObject obj, string itemName)
	{
		photonView.RPC("SyncReturnObject", RpcTarget.All, obj.GetPhotonView().ViewID, itemName);
	}

	[PunRPC]
	private void SyncReturnObject(int viewID, string itemName)
	{
        GameObject obj = PhotonView.Find(viewID).gameObject;

        if (!pool.ContainsKey(itemName))
		{
			pool[itemName] = new Queue<GameObject>();
		}
		obj.SetActive(false);
		pool[itemName].Enqueue(obj);
	}
}
