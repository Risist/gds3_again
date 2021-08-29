using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * Simple class to manage multi key cheat commands
 */
public class CommandManager : MonoSingleton<CommandManager>
{
    static public new CommandManager Instance => GetInstance(
        autoCreateReference: true,
        findReference: true,
        logErrorIfNotResolved: true);

    struct CommandRecord
    {
        public CheatPasswordChecker passwordChecker;
        public Action commandAction;
        public int commandId;
        public bool oneTime;
    }

    // adds command to currently executed
    public int AddCommand(KeyCode[] password, Action action, bool oneTime = false)
    {
        var command = new CommandRecord();
        command.passwordChecker = new CheatPasswordChecker(password);
        command.commandAction = action;
        command.commandId = nextAddedCommandId++;
        command.oneTime = oneTime;

        commands.Add(command);

        return command.commandId;
    }

    public void RemoveCommand(int commandId)
    {
        commands.RemoveAll((c) => c.commandId == commandId);
    }

    List<CommandRecord> commands = new List<CommandRecord>();
    int nextAddedCommandId = 0;

    private void Update()
    {
        for(int i = 0; i < commands.Count; ++i)
        {
            ProcessCommand(i);
        }
    }
    void ProcessCommand(int i)
    {
        var it = commands[i];
        it.passwordChecker.CheckPassword();

        if (it.passwordChecker.passwordAccepted)
        {
            it.commandAction();

            if (it.oneTime)
                commands.RemoveAt(i);
            else
                it.passwordChecker.ResetPassword();
        }
    }
}
