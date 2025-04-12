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
    private bool isInitialized = false;

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
        random = new System.Random();
        passwords = new List<string>();
        validPasswords = new List<string>();
        validatedPassword = "";

        if (PhotonNetwork.IsMasterClient)
        {
            EventManager_Game.Instance.OnAllPlayerSpawned += OnAllPlayerSpawned;
        }
    }

    private void OnDestroy()
    {
        if (EventManager_Game.Instance != null && PhotonNetwork.IsMasterClient)
        {
            EventManager_Game.Instance.OnAllPlayerSpawned -= OnAllPlayerSpawned;
        }
    }

    private void OnAllPlayerSpawned()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        GeneratePasswords(playerCount);
        InitPasswords();
        isInitialized = true;
    }

    private void GeneratePasswords(int playerCount)
    {
        int count = 0;
        while (count < playerCount * 3)
        {
            string newPassword = GenerateRandomPassword();
            if (!passwords.Contains(newPassword))
            {
                passwords.Add(newPassword);

                if (count < playerCount * 2)
                {
                    validPasswords.Add(newPassword);
                }
                count++;
            }
        }
    }

    private void InitPasswords()
    {
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

        isInitialized = true;
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
        if (!isInitialized)
        {
            Debug.LogWarning("PasswordManager: 비밀번호가 아직 초기화되지 않았습니다.");
            return null;
        }

        if (setterIdx >= passwords.Count)
        {
            throw new InvalidOperationException("PasswordGenerator에서 더 이상 설정할 수 있는 비밀번호가 없습니다.");
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
        EventManager_Game.Instance?.InvokeOnePasswordValid();
    }

    public bool SecondValidatePassword(string inputPassword)
    {
        int validatedIndex = validPasswords.IndexOf(validatedPassword);
        int inputPasswordIndex = validPasswords.IndexOf(inputPassword);

        if (validatedIndex < 0 || inputPasswordIndex < 0) return false;

        if (validatedIndex > inputPasswordIndex)
        {
            validPasswords.RemoveAt(validatedIndex);
            validPasswords.RemoveAt(inputPasswordIndex);
        }
        else if (validatedIndex < inputPasswordIndex)
        {
            validPasswords.RemoveAt(inputPasswordIndex);
            validPasswords.RemoveAt(validatedIndex);
        }
        else
        {
            validPasswords.RemoveAt(validatedIndex);
        }

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

    [PunRPC]
    private void OnAllPasswordsMatched()
    {
        validPasswords = null;
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

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
