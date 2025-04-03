using System;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class PasswordManager : MonoBehaviourPun
{
    public static PasswordManager Instance { get; private set; }

    [SerializeField] private List<string> passwords;
    [SerializeField] private List<string> validPasswords;
    [SerializeField] private string validatedPassword;

    private System.Random random;
    private int setterIdx = 0;
    private int pairingCount;

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

    private void Start()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        pairingCount = playerCount / 2 + 1;
        random = new System.Random();
        passwords = new List<string>();
        validPasswords = new List<string>();
        validatedPassword = "";

        if (PhotonNetwork.IsMasterClient)
        {
            GeneratePasswords(playerCount);
        }
    }

    private void GeneratePasswords(int playerCount)
    {
        int count = 0;
        while (count < playerCount * 2)
        {
            string newPassword = GenerateRandomPassword();
            if (!passwords.Contains(newPassword))
            {
                passwords.Add(newPassword);

                if (count < pairingCount * 2)
                {
                    validPasswords.Add(newPassword);
                }
                count++;
            }
        }
        photonView.RPC("SyncInitPasswords", RpcTarget.All, passwords.ToArray(), validPasswords.ToArray());
    }

    [PunRPC]
    private void SyncInitPasswords(string[] _passwords, string[] _validPasswords)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            passwords = new List<string>(_passwords);
            validPasswords = new List<string>(_validPasswords);
        }
    }

    private string GenerateRandomPassword()
    {
        char[] password = new char[6];
        for (int i = 0; i < 6; i++)
        {
            password[i] = (char)('0' + random.Next(0, 10));
        }
        return new string(password);
    }

    public string SetPasswordToPaper()
    {
        if (setterIdx >= passwords.Count)
        {
            throw new InvalidOperationException("PasswordGenerator에서 더 이상 가져올 수 있는 패스워드가 없습니다.");
        }
        return passwords[setterIdx++];
    }

    public bool ValidatePassword(string inputPassword)
    {
        if (validatedPassword == "")
        {
            return FirstValidatePassword(inputPassword);
        }
        else
        {
            return SecondValidatePassword(inputPassword);
        }
    }

    public bool FirstValidatePassword(string inputPassword)
    {
        if (validPasswords.Contains(inputPassword))
        {
            validatedPassword = inputPassword;
            photonView.RPC("SyncValidatedPassword", RpcTarget.All, validatedPassword);
            return true;
        }
        return false;
    }

    [PunRPC]
    private void SyncValidatedPassword(string _validatedPassword)
    {
        validatedPassword = _validatedPassword;
    }

    public bool SecondValidatePassword(string inputPassword)
    {
        int validatedIndex = validPasswords.IndexOf(validatedPassword);
        if (validatedIndex < 0) return false;

        bool validated = false;

        if (IsEven(validatedIndex) && ComparePasswordWithIndex(validatedIndex + 1, inputPassword))
        {
            validPasswords.RemoveAt(validatedIndex);
            validPasswords.RemoveAt(validatedIndex);
            validated = true;
        }
        else if (!IsEven(validatedIndex) && ComparePasswordWithIndex(validatedIndex - 1, inputPassword))
        {
            validPasswords.RemoveAt(validatedIndex);
            validPasswords.RemoveAt(validatedIndex - 1);
            validated = true;
        }

        if (validated)
        {
            if (validPasswords.Count == 0)
            {
                photonView.RPC("OnAllPasswordsMatched", RpcTarget.All);
            }
            else
            {
                photonView.RPC("SyncRemovePassword", RpcTarget.All, validPasswords.ToArray());
            }
            ResetValidatedPassword();
            return true;
        }
        return false;
    }

    [PunRPC]
    private void OnAllPasswordsMatched()
    {
        validPasswords = null;
    }

    private bool ComparePasswordWithIndex(int compareIndex, string inputPassword)
    {
        if (validPasswords[compareIndex] == inputPassword)
        {
            return true;
        }
        return false;
    }

    private bool IsEven(int validatedIndex)
    {
        return (validatedIndex % 2 == 0);
    }

    [PunRPC]
    private void SyncRemovePassword(string[] _validPasswords)
    {
        validPasswords = new List<string>(_validPasswords);
    }

    public void ResetValidatedPassword()
    {
        photonView.RPC("SyncValidatedPassword", RpcTarget.All, "");
    }
}
