using Godot;
using System;

public partial class ScriptConroller : DialogueController
{
    public override void _Ready()
    {
        if(LevelController.gameFlag.dialogueFlag!=null)
        {
            StartNormalDialogue(LevelController.gameFlag.dialogueFlag);
        }
    }
}
