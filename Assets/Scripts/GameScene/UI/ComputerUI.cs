using UnityEngine;
using TMPro;
using System.Collections;

public class ComputerUI : MonoBehaviour
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

    public void HandleTypeNumber(char keyCode)
	{
		if (tmp.text.Length < 6)
		{
            tmp.text += keyCode;
        }
		if (tmp.text.Length == 6)
		{
			CompareWithValidPassword(tmp.text);
        }

	}

	private void CompareWithValidPassword(string typedPassword)
	{
		tmp.text = "";
	}

	public void HandleTypeBackspace()
	{
		if (tmp.text.Length > 0)
		{
			tmp.text = tmp.text.Substring(0, tmp.text.Length - 1);
		}
	}


}
