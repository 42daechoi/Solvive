using System.Collections;
using TMPro;
using UnityEngine;

public class WaitForSpawnUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    private IEnumerator WaitForEventManager()
    {
        while (EventManager_Game.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        EventManager_Game.Instance.OnAllPlayerSpawned += InactiveWaitForSpawnUI;

    }

    private void OnEnable()
    {
        StartCoroutine(WaitForEventManager());
    }

    private void OnDisable()
    {
        EventManager_Game.Instance.OnAllPlayerSpawned -= InactiveWaitForSpawnUI;
    }

    public void InactiveWaitForSpawnUI()
    {
        tmp.enabled = false;
    }
}