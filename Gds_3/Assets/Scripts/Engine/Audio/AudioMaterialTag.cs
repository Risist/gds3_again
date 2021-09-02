using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tag", menuName = "Ris/Audio/Tag")]
public class AudioMaterialTag : ScriptableObject
{
    public struct RelationEvent
    {
        public AudioMaterialTag toTag;
        public AK.Wwise.Event audioEvent;
    }

    public struct RelationSwitch
    {
        public AudioMaterialTag toTag;
        public AK.Wwise.Switch audioEvent;
    }

    public RelationEvent[] relationEvents;
    public RelationSwitch[] relationSwitches;

    //public AK.Wwise.Event GetEvent()
}
