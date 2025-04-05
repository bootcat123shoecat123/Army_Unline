using DialogueManagerRuntime;
using Godot;
using System;

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
        StartNormalDialogue(LevelController.gameFlag.dialogueFlag);

    }
    public void StartNormalDialogue(Resource dialogue,string key="")
    {
        DialogueManager.ShowDialogueBalloon(dialogue,key);
    }
    public void StartPlayScene(){
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
       
    }
}
