using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;

public class ComputerUI : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI tmp;
    private IEnumerator WaitForEventManager()
    {
        while (EventManager_Game.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EventManager_Game.Instance.OnTypeBackspaceAtComputer += HandleTypeBackspace;
        EventManager_Game.Instance.OnTypeNumberAtComputer += HandleTypeNumber;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForEventManager());
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnTypeBackspaceAtComputer -= HandleTypeBackspace;
        EventManager_Game.Instance.OnTypeNumberAtComputer -= HandleTypeNumber;
    }

    public void HandleTypeNumber(char keyCode, int playerID)
    {
        if (TryGetComponent(out Computer computer))
        {
            if (computer.GetUsingPlayerID() == playerID)
            {
                if (tmp.text.Length < 6)
                {
                    tmp.text += keyCode;
                }
                photonView.RPC("UpdateText", RpcTarget.All, tmp.text);
                if (tmp.text.Length == 6)
                {
                    ComparePassword(tmp.text);
                }
            }
        }
    }

    private void ComparePassword(string password)
    {
        photonView.RPC("UpdateText", RpcTarget.All, "");
    }

    [PunRPC]
    public void UpdateText(string newText)
    {
        tmp.text = newText;
    }

    public void HandleTypeBackspace(int playerID)
    {
        if (TryGetComponent(out Computer computer))
        {
            if (computer.GetUsingPlayerID() == playerID)
            {
                if (tmp.text.Length > 0)
                {
                    tmp.text = tmp.text.Substring(0, tmp.text.Length - 1);
                }

                photonView.RPC("UpdateText", RpcTarget.All, tmp.text);
            }
        }
    }
}
