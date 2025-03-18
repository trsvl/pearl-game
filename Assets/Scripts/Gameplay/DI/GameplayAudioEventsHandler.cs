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
        public GameplayAudioEventsHandler(AudioController audioController, List<Audios> audios,
            CancellationToken cancellationToken) : base(audioController, audios, cancellationToken)
        {
        }

        public void OnReleaseBall()
        {
            AudioController.Play(AudioAction.Throw);
        }

        public void OnDestroySphereSegmentOnHit(Color segmentColor, GameObject target, int currentShotsNumber)
        {
            AudioController.Play(AudioAction.HitSphere);
        }

        public void OnSpawnSphereSegment()
        {
            AudioController.Play(AudioAction.SpawnSphereSegment);
        }

        public void PauseGame()
        {
            AudioController.PauseGame();
        }

        public void ResumeGame()
        {
            AudioController.ResumeGame();
        }

        public void StartGame()
        {
            AudioController.Play(AudioAction.StartGame);
        }

        public void FinishGame()
        {
            AudioController.Play(AudioAction.FinishGame);
        }

        public void LoseGame()
        {
            AudioController.Play(AudioAction.LoseGame);
        }

        public void OnDestroySphereSegment()
        {
            AudioController.Play(AudioAction.HitSphereSegment);
        }
    }
}