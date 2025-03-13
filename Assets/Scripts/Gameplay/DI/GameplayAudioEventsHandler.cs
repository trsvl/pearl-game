using System.Collections.Generic;
using System.Threading;
using Bootstrap;
using UnityEngine;
using Utils.Scene.AudioSystem;

namespace Gameplay.DI
{
    public class GameplayAudioEventsHandler : AudioEventsHandler, IReleaseBall, IDestroySphereSegmentOnHit,
        ISpawnSphereSegment, IPauseGame, IResumeGame, IStartGame, IFinishGame, ILoseGame, IDestroySphereSegment
    {
        public GameplayAudioEventsHandler(AudioManager audioManager, List<Audios> audios,
            CancellationToken cancellationToken) : base(audioManager, audios, cancellationToken)
        {
        }

        public void OnReleaseBall()
        {
            _audioManager.Play(AudioAction.Throw);
        }

        public void OnDestroySphereSegmentOnHit(Color segmentColor, GameObject target, int currentShotsNumber)
        {
            _audioManager.Play(AudioAction.HitSphere);
        }

        public void OnSpawnSphereSegment()
        {
            _audioManager.Play(AudioAction.SpawnSphereSegment);
        }

        public void PauseGame()
        {
            _audioManager.PauseGame();
        }

        public void ResumeGame()
        {
            _audioManager.ResumeGame();
        }

        public void StartGame()
        {
            _audioManager.Play(AudioAction.StartGame);
        }

        public void FinishGame()
        {
            _audioManager.Play(AudioAction.FinishGame);
        }

        public void LoseGame()
        {
            _audioManager.Play(AudioAction.LoseGame);
        }

        public void OnDestroySphereSegment()
        {
            _audioManager.Play(AudioAction.HitSphereSegment);
        }
    }
}