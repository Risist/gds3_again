using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BarkSystem
{
    [CreateAssetMenu(fileName = "New Bark Set", menuName = "Ris/Ai/BarkSet", order = 0)]
    public class BarkSet : ScriptableObject
    {
        public List<BarkRecord> records;
    }

    [Serializable]
    public class BarkRecord
    {
        public string text = "";
        public float utility = 1.0f;
        public float barkDisplayTime = 1.0f;
    }
}
