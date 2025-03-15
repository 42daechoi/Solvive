using TMPro;
using UnityEngine;

public class RoleUI : MonoBehaviour
{
    public static RoleUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI roleText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void UpdateRoleUI(PlayerRole playerRole)
    {
        string roleName = "";
        if (playerRole == PlayerRole.Observer)
        {
            roleName = "Role : Observer";
        }
        else if (playerRole == PlayerRole.Citizen)
        {
            roleName = "Role : Civilian";
        }
        else if (playerRole == PlayerRole.Mannequin)
        {
            roleName = "Role : Mannequin";
        }

        roleText.text = roleName;
    }
}
