using System;
using System.Collections.Generic;
using System.Threading;
using Bootstrap;
using UnityEngine;

namespace Utils.Scene.AudioSystem
{
    public class AudioEventsHandler : IDisposable
    {
        protected readonly AudioManager _audioManager;
        private readonly List<Audios> _audios;


        protected AudioEventsHandler(AudioManager audioManager, List<Audios> audios,
            CancellationToken cancellationToken)
        {
            _audioManager = audioManager;
            _audios = audios;

            _audioManager.AssignNewToken(cancellationToken);
            _audioManager.SubscribeAudio(_audios);
        }

        public void Dispose()
        {
            Debug.Log("AudioEventsHandler.Dispose");
            _audioManager.UnsubscribeAudio(_audios);
        }
    }
}