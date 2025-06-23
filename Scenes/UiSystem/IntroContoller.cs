using ArmyUnline.Scripts.Enums;
using Godot;
using Scripts.Enums;
using System;

public partial class IntroContoller : Node2D
{
    AnimationPlayer animationPlayer;
    Node buttonGroup;
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AudioSystem.audioSystemInstance.PlayAudio(AudioType.BGM,"翔びゆく雲、夏が往く",true);
        animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        buttonGroup = GetNode<Node>("%TitleButton");
    }
    public override void _Process(double delta)
    {
    }
    private void EndGame()
    {
        GetTree().Quit();
    }

    private void StartGame()
    {

        foreach (Button nowButton in buttonGroup.GetChildren())
        {

            if (nowButton.Disabled) return;
            nowButton.Disabled = true;
        }
        animationPlayer.Play("fade_in");
    }
    private void Setting()
    {
        GD.Print(buttonGroup.Name);
    }
    private void OnAnimationPlayerAnimationFinished(string animationName)
    {

        
        if (animationName.Equals("fade_in"))
        {         
            LevelController.gameFlagState=new GameFlagState();
            LevelController.gameFlagState.gameFlag=GameFlagEnums.Intro001;
            LevelController.levelControllerInstance.LoadLevel(SceneTypes.Script);
        }
           // LevelController.levelController.LoadLevel(SceneTypes.Script);
    }
}
