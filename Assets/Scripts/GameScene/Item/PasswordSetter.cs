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
        PasswordGenerator passwordGenerator = GameManager.Instance.GetPasswordGenerator();
        if (passwordGenerator == null)
        {
            Debug.LogError("PasswordSetter : PasswordGenerator가 null입니다.");
            return;
        }

        try
        {
            password = passwordGenerator.SetPasswordToPaper();
        }
        catch (InvalidOperatorException e)
        {
            Debug.LogError($"PasswordSetter : {e.Message}");
        }
        tmp.text = password;
    }
}
