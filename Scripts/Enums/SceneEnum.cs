using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmyUnline.Scripts.Enums
{

    public enum SceneTypes
    {
        Script,
        Live,
        HScene,
        War
    }
    [Flags]
    public enum AudioType
    {
        BGM=0,
        SFX=1,
        HSE=2,
        Voice=3
    }
}
