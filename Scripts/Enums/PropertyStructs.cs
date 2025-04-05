using ArmyUnline.Scripts.Enums;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Enums
{
    public struct AudioSorce
    {
        public AudioStreamMP3 audioSorce;
        public AudioType type;    
    }
    
    public struct GameFlag
    {
        public Resource dialogueFlag;
        public SceneTypes runSceneType;
        
    }
    public struct TalkSceneState
    {
        public string characterName;
        public string dialogueID;
        public string background;
        public string mood;
        public string action;
    }
}
