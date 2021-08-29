using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class CheatPasswordChecker
{
    public CheatPasswordChecker(KeyCode[] password)
    {
        cheatPassword = password;
    }

    KeyCode[] cheatPassword = new KeyCode[]
    {
        KeyCode.J,
        KeyCode.E,
        KeyCode.B,
        KeyCode.A,
        KeyCode.C,
        KeyCode.Space,
        KeyCode.U,
        KeyCode.N,
        KeyCode.I,
        KeyCode.T,
        KeyCode.Y,
    };

    int passwordCharacterId;
    public bool passwordAccepted { get; private set; }
    public void ResetPassword()
    {
        passwordAccepted = false;
        passwordCharacterId = 0;
    }

    public void CheckPassword()
    {
        if (passwordAccepted)
            return;

        if (Input.GetKeyDown(cheatPassword[passwordCharacterId]))
        {
            ++passwordCharacterId;
            if (passwordCharacterId >= cheatPassword.Length)
            {
                passwordAccepted = true;
            }
        }
        else if (Input.anyKeyDown)
        {
            passwordCharacterId = 0;
        }
    }
}