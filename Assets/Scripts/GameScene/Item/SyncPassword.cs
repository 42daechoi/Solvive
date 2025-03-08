using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SyncPassword : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    public void SetPasswordToFirstPersonPaper(GameObject thirdPersonPasswordPaper)
    {
        PasswordSetter passwordSetter = thirdPersonPasswordPaper.GetComponent<PasswordSetter>();
        if (passwordSetter == null) return;


        tmp.text = passwordSetter.GetPassword();
    }
}
