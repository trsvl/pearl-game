using System.Collections.Generic;
using UnityEngine;

namespace Utils.Scene.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioList", menuName = "Audio System/Audio List", order = 1)]
    public class AudioList : ScriptableObject
    {
        public List<Audios> audios;
    }
}