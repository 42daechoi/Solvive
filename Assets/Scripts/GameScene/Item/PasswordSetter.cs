using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PasswordSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    private string password;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PasswordSetter : GameManager 싱글톤이 null입니다.");
            return;
        }
        PasswordManager passwordManager = PasswordManager.Instance;
        if (passwordManager == null)
        {
            Debug.LogError("PasswordSetter : PasswordGenerator가 null입니다.");
            return;
        }

        try
        {
            password = passwordManager.SetPasswordToPaper();
        }
        catch (InvalidOperatorException e)
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
