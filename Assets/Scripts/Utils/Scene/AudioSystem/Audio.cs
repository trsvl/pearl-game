using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Utils.Scene.AudioSystem
{
    [Serializable]
    public class Audios
    {
        public AudioAction action;
        public Audio[] audios;
    }

    [Serializable]
    public class Audio
    {
        [HideInInspector] public AudioSource source;
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        [Range(0, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
        public bool playOnAwake = false;
        public bool loop = false;
        public float fadeInDuration;
        public float fadeOutDuration;
        public float startTime;
        public float delay;
    }
}