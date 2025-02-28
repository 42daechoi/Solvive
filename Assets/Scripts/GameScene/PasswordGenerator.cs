using System;
using System.Collections.Generic;

public class PasswordGenerator
{
    private string[] passwords { get; set; }
    private List<string> validPasswords { get; set; }
    private Random random;
    private int setterIdx = 0;
    public string[] GetPasswords() => passwords;
    public string[] GetValidPasswords()
    {
        return validPasswords.ToArray();
    }

    public void SetPasswords(string[] newPasswords)
    {
        passwords = newPasswords;
    }

    public void SetValidPasswords(List<string> newValidPasswords)
    {
        validPasswords = newValidPasswords;
    }
    public PasswordGenerator()
    {
        random = new Random();
        passwords = new string[10];
        validPasswords = new List<string>();

        GeneratePasswords();
    }

    private void GeneratePasswords()
    {
        int count = 0;
        while (count < passwords.Length)
        {
            string newPassword = GenerateRandomPassword();
            if (Array.IndexOf(passwords, newPassword) == -1)
            {
                passwords[count] = newPassword;
                count++;
                if (validPasswords.Count < 2)
                {
                    validPasswords.Add(newPassword);
                }
            }
        }

        //while (validPasswords.Count < 2)
        //{
        //    int index = random.Next(0, passwords.Length);
        //    validPasswords.Add(passwords[index]);
        //}
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
        if (setterIdx >= passwords.Length)
        {
            throw new InvalidOperationException("PasswordGenerator에서 더 이상 가져올 수 있는 패스워드가 없습니다.");
        }
        return passwords[setterIdx++];
    }

    public bool ValidatePassword(string inputPassword)
    {
        return validPasswords.Contains(inputPassword);
    }

}
