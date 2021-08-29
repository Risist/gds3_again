using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CheatManager : MonoSingleton<CheatManager>
{
    static public new CheatManager Instance => GetInstance(
        autoCreateReference: true,
        findReference: true,
        logErrorIfNotResolved: true);

    public static Action<KeyCode> onCheatButtonPressed = (key) => { };
    const KeyCode cheatKey = KeyCode.K;

    private void Start()
    {
        /*CommandManager.Instance.AddCommand(new KeyCode[]
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
            }, 
            () => enabled = true 
        );

        enabled = false;*/
    }
    void Update()
    {
        if (!Input.GetKey(cheatKey))
            return;

        IEnumerable<KeyCode> keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>();

        foreach (var keyCode in keyCodes)
            if (cheatKey != keyCode && Input.GetKeyDown(keyCode))
        {
            onCheatButtonPressed(keyCode);
        }
    }

}
