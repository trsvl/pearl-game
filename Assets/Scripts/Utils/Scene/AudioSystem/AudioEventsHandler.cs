using System;
using System.Collections.Generic;
using System.Threading;
using Bootstrap;
using UnityEngine;

namespace Utils.Scene.AudioSystem
{
    public class AudioEventsHandler : IDisposable
    {
        protected readonly AudioController AudioController;
        private readonly List<Audios> _audios;


        protected AudioEventsHandler(AudioController audioController, List<Audios> audios,
            CancellationToken cancellationToken)
        {
            AudioController = audioController;
            _audios = audios;

            AudioController.AssignNewToken(cancellationToken);
            AudioController.SubscribeAudio(_audios);
        }

        public void Dispose()
        {
            AudioController.UnsubscribeAudio(_audios);
        }
    }
}