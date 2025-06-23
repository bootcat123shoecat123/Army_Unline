using Godot;
using System;
using DialogueManagerRuntime;
using GodotResourceGroups;

public partial class ScriptConroller : DialogueController
{
    public override void _Ready()
    {
        if(LevelController.gameFlagState.gameFlag!=null)
        {
            StartNormalDialogue(LevelController.gameFlagState.gameFlag);
        }
        DialogueManager.DialogueEnded +=  NextScene;
    }
    public void NextScene(Resource dialogueResource)
    {
        
    }
    public void LoadDialogueResorces(){
var resourceGroup = ResourceGroup.Of("res://path/to/resource_group.tres");

    }
}
