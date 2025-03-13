using System;
using System.Collections.Generic;
using System.Threading;
using Bootstrap;

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
            _audioManager.UnsubscribeAudio(_audios);
        }
    }
}