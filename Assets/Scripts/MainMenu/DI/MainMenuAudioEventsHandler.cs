using System.Collections.Generic;
using System.Threading;
using Bootstrap;
using Utils.Scene.AudioSystem;

namespace MainMenu.DI
{
    public class MainMenuAudioEventsHandler : AudioEventsHandler, IMainMenuStart
    {
        public MainMenuAudioEventsHandler(AudioController audioController, List<Audios> audios,
            CancellationToken cancellationToken) : base(audioController, audios, cancellationToken)
        {
        }

        public void OnMainMenuStart()
        {
            AudioController.Play(AudioAction.MainMenuStart);
        }
    }
}