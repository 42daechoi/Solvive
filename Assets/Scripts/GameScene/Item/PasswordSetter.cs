using TMPro;
using UnityEngine;

public class PasswordSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    private string password;

    private void Start()
    {
        StartCoroutine(WaitForPasswordManagerInitialized());
    }

    private System.Collections.IEnumerator WaitForPasswordManagerInitialized()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PasswordSetter : GameManager 싱글톤이 null입니다.");
            yield break;
        }

        PasswordManager passwordManager = PasswordManager.Instance;

        if (passwordManager == null)
        {
            Debug.LogError("PasswordSetter : PasswordManager가 null입니다.");
            yield break;
        }

        yield return new WaitUntil(() => passwordManager.IsInitialized());

        try
        {
            password = passwordManager.SetPasswordToPaper();
        }
        catch (System.InvalidOperationException e)
        {
            Debug.LogError($"PasswordSetter : {e.Message}");
        }

        tmp.text = password;
    }

    public string GetPassword()
    {
        return password;
    }
}
