using Cos.Audio;
using Cos.Library;
using System.IO;

namespace Cos.Engine.Manager
{
    /// <summary>
    /// /// Contains global sounds and music.
    /// </summary>
    public class AudioManager
    {
        private readonly EngineCore _engine;

        public CosAudioClip BackgroundMusicSound { get; private set; }
        public CosAudioClip Click { get; private set; }

        public AudioManager(EngineCore engine)
        {
            _engine = engine;

            Click = _engine.Assets.GetAudio(@"Sounds\Other\Click.wav", 0.70f, false);
            BackgroundMusicSound = _engine.Assets.GetAudio(@"Sounds\Music\Background.wav", 0.25f, true);
        }

        public void PlayRandomShieldHit()
        {
            var audioClip = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Hit.wav", 1.0f);
            audioClip?.Play();
        }

        public void PlayRandomHullHit()
        {
            var audioClip = _engine.Assets.GetAudio(@"Sounds\Ship\Object Hit.wav", 1.0f);
            audioClip?.Play();
        }

        public void PlayRandomExplosion()
        {
            const string assetPath = @"Sounds\Explode\";
            int assetCount = 4;
            int selectedAssetIndex = CosRandom.Between(0, assetCount - 1);
            var audioClip = _engine.Assets.GetAudio(Path.Combine(assetPath, $"{selectedAssetIndex}.wav"), 1.0f);
            audioClip?.Play();
        }
    }
}
