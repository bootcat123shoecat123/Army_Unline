using ArmyUnline.Scripts.Enums;
using DialogueManagerRuntime;
using Godot;
using System;
using System.ComponentModel;
using System.Reflection;

public partial class DialogueController : Node
{
    // Called when the node enters the scene tree for the first time.
    [Export] PackedScene SmallBalloon;
    PackedScene Balloon=GD.Load<PackedScene>("res://UI/TalkBalloon/BaseBalloon.tscn");
   
    //Audio main
    //string nowPlayingKey
    public Node NowBalloon { get; set; }

    public override void _Ready()
    {
        string path = LevelController.gameFlagState.gameFlag.GetType()
            .GetField(LevelController.gameFlagState.gameFlag.ToString())
            .GetCustomAttribute<DescriptionAttribute>()
            .Description;
        StartNormalDialogue(LevelController.gameFlagState.gameFlag);
    }
    public void StartNormalDialogue(GameFlagEnums gameFlag,string key="")
    {
        string path = gameFlag.GetType()
            .GetField(gameFlag.ToString())
            .GetCustomAttribute<DescriptionAttribute>()
            .Description;

        DialogueManager.ShowDialogueBalloon(GD.Load<Resource>(path),key);
    }
    public void StartPlayScene(){
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
       
    }
}
