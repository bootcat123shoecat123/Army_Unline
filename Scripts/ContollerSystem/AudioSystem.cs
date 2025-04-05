using ArmyUnline.Scripts.Enums;
using Godot;
using System;
using System.Linq;
using Godot.Collections;
using Scripts.Enums;

public partial class AudioSystem : Node
{
	public static AudioSystem audioSystemInstance;
    private readonly Dictionary<AudioType, Array<AudioStreamMP3>> songList = new();
    private readonly Dictionary<AudioType, Array<AudioStreamPlayer>> playingSongs = new();

	[Export] public Dictionary<AudioType,float> volumeList = new();

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
		volumeList[AudioType.BGM] = 1.0f;
		volumeList[AudioType.SFX] = 1.0f;
		volumeList[AudioType.HSE] = 1.0f;
		volumeList[AudioType.Voice] = 1.0f;
        LoadAllAudioFiles();
		audioSystemInstance = this;
    }

    private void LoadAllAudioFiles()
    {
        string basePath = "res://Sound";
        var dir = DirAccess.Open(basePath);
        
        if (dir != null)
        {
            // 遍历所有AudioType文件夹
            foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
            {
                // 初始化音量列表
                if (!volumeList.ContainsKey(audioType))
                {
                    volumeList[audioType] = 0.0f; // 默认音量为0dB
                }

                string typePath = $"{basePath}/{audioType}";
                var typeDir = DirAccess.Open(typePath);
                
                if (typeDir != null)
                {
                    songList[audioType] = new Array<AudioStreamMP3>();
                    playingSongs[audioType] = new Array<AudioStreamPlayer>();

                    // 遍历该类型下的所有mp3文件
                    typeDir.ListDirBegin();
                    string fileName = typeDir.GetNext();
                    while (!string.IsNullOrEmpty(fileName))
                    {
                        if (!fileName.StartsWith(".") && fileName.EndsWith(".mp3"))
                        {
                            string fullPath = $"{typePath}/{fileName}";
                            var audio = ResourceLoader.Load<AudioStreamMP3>(fullPath);
                            if (audio != null)
                            {
                                songList[audioType].Add(audio);
                                GD.Print($"Loaded audio: {fullPath}");
                            }
                        }
                        fileName = typeDir.GetNext();
                    }
                    typeDir.ListDirEnd();
                }
                else
                {
                    GD.Print($"Could not access directory: {typePath}");
                }
            }
        }
        else
        {
            GD.PrintErr($"Could not access base audio directory: {basePath}");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PlayAudio(AudioType audioType, string audioName,bool isLoop=false)
    {

        if (!songList.ContainsKey(audioType))
        {
            GD.PrintErr($"Audio type not found: {audioType}");
            return;
        }

		if(playingSongs[audioType].FirstOrDefault(player=>player.Name == audioName) != null){
			GD.Print($"Audio already playing: {audioName}");
			return;
		}
		

        var audio = songList[audioType].FirstOrDefault(song => 
            song.ResourcePath.GetFile().Replace(".mp3", "") == audioName);
		
		if(audioType ==AudioType.BGM){
			StopAllAudio(AudioType.BGM);
		}
		if(audioType ==AudioType.HSE){
			StopAllAudio(AudioType.HSE);
		}
		

        if (audio != null)
        {
            var player = new AudioStreamPlayer();
            player.Stream = audio;
            player.VolumeDb = volumeList[audioType];
            AddChild(player);
            player.Play();
            playingSongs[audioType].Add(player);
            if(isLoop)
            {
                player.Autoplay = true;
            }else{
                player.Finished += () =>
                {
                    playingSongs[audioType].Remove(player);
                    player.QueueFree();
                };
            }
        }
        else
        {
            GD.PrintErr($"Audio not found: {audioName} in {audioType}");
        }

    }

    public void SetVolume(AudioType audioType, float volume)
    {
        if (!playingSongs.ContainsKey(audioType))
        {
            return;
        }

        foreach (var player in playingSongs[audioType])
        {
            player.VolumeDb = volume;
        }
		volumeList[audioType] = volume;
    }

    public void PauseAudio(AudioType audioType,string audioName)
    {
        if (!playingSongs.ContainsKey(audioType))
        {
			GD.PrintErr($"Audio type not found: {audioType}");
            return;
        }
		if(playingSongs[audioType].FirstOrDefault(player=>player.Name == audioName) == null){
			GD.PrintErr($"Audio not found: {audioName} in {audioType}");
            return;
		}
        var player = playingSongs[audioType].FirstOrDefault(player=>player.Name == audioName);
        if(player != null)
        {
            player.StreamPaused = true;
        }
    }
    public void StopAllAudio(AudioType audioType)
    {
        if (!playingSongs.ContainsKey(audioType))
        {
            return;
        }

        foreach (var player in playingSongs[audioType])
        {
            player.Stop();
            player.QueueFree();
        }
        playingSongs[audioType].Clear();
    }
}
