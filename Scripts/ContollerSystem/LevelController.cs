using Godot;
using System;
using System.Xml.Schema;
using ArmyUnline.Scripts.Enums;
using System.IO;
using Scripts.Enums;
public partial class LevelController:Node
{
	public static Node CurrentScene;
    public static GameFlag gameFlag;
    public static LevelController levelControllerInstance { get; private set; }
                                                  // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        levelControllerInstance=this;
        Viewport root = GetTree().Root;
        // Using a negative index counts from the end, so this gets the last child node of `root`.
        CurrentScene = root.GetChild(-1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void DeferredGotoScene(string path)
    {
        // It is now safe to remove the current scene.
        CurrentScene.Free();

        // Load a new scene.
        var nextScene = GD.Load<PackedScene>(path);

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        GetTree().Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        GetTree().CurrentScene = CurrentScene;
    }
    public void LoadLevel(SceneTypes scene)
	{
		GD.Print("Loading");
		switch (scene)
		{
            case SceneTypes.Script:
                gameFlag.runSceneType=SceneTypes.Script;
                CallDeferred(MethodName.DeferredGotoScene, "res://Scenes/GameLevel/ScriptScene.tscn");
                
				return;
			case SceneTypes.Live:
                gameFlag.runSceneType = SceneTypes.Live;
                break;
			case SceneTypes.HScene:
                gameFlag.runSceneType = SceneTypes.HScene;
                break;
			case SceneTypes.War:
                gameFlag.runSceneType = SceneTypes.War;
                break;	
			default:
				break;
		}
		//

	}
}