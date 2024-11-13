using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace Cos.Audio
{
    /// <summary>
    /// A single pre-loaded audio-clip.
    /// </summary>
    public class CosAudioClip
    {
        private readonly XAudio2 _xaudio = new();
        private readonly WaveFormat _waveFormat;
        private readonly AudioBuffer _buffer;
        private readonly SoundStream _soundStream;
        private SourceVoice? _singleSourceVoice;
        private bool _loopForever;
        private bool _isPlaying = false; //Only applicable when _loopForever == false;
        private bool _isFading;
        public float InitialVolume { get; private set; }

        public void SetVolume(float volume)
        {
            _singleSourceVoice?.SetVolume(volume);
        }

        public void SetInitialVolume(float volume)
        {
            InitialVolume = volume;
        }

        public void SetLoopForever(bool loopForever)
        {
            _loopForever = loopForever;
        }

        public CosAudioClip(Stream stream, float initialVolume = 1, bool loopForever = false)
        {
            _loopForever = loopForever;
            InitialVolume = initialVolume;

            _ = new MasteringVoice(_xaudio); //Yes, this is required.

            _soundStream = new SoundStream(stream);

            _waveFormat = _soundStream.Format;
            _buffer = new AudioBuffer
            {
                Stream = _soundStream.ToDataStream(),
                AudioBytes = (int)_soundStream.Length,
                Flags = BufferFlags.EndOfStream,
            };

            if (loopForever)
            {
                _buffer.LoopCount = 100;
            }
        }

        public void Play()
        {
            lock (this)
            {
                if (_loopForever == true)
                {
                    if (_isPlaying)
                    {
                        if (_isFading)
                        {
                            _isFading = false;
                            _singleSourceVoice?.SetVolume(InitialVolume);
                        }

                        return;
                    }

                    _singleSourceVoice = new SourceVoice(_xaudio, _waveFormat, true);
                    _singleSourceVoice.SubmitSourceBuffer(_buffer, _soundStream.DecodedPacketsInfo);
                    _singleSourceVoice.SetVolume(InitialVolume);
                    _singleSourceVoice.Start();
                    _isPlaying = true;
                    return;
                }
            }

            var sourceVoice = new SourceVoice(_xaudio, _waveFormat, true);
            sourceVoice.SubmitSourceBuffer(_buffer, _soundStream.DecodedPacketsInfo);
            sourceVoice.SetVolume(InitialVolume);
            sourceVoice.Start();
        }

        public void Fade()
        {
            if (_isPlaying && _isFading == false)
            {
                _isFading = true;
                new Thread(FadeThread).Start();
            }
        }

        private void FadeThread()
        {
            float volume;

            if (_singleSourceVoice != null)
            {
                _singleSourceVoice.GetVolume(out volume);

                while (_isFading && volume > 0)
                {
                    volume -= 0.25f;
                    volume = volume < 0 ? 0 : volume;
                    _singleSourceVoice.SetVolume(volume);
                    Thread.Sleep(100);
                }
                Stop();
            }
        }

        public void Stop()
        {
            if (_loopForever == true)
            {
                if (_singleSourceVoice != null && _isPlaying)
                {
                    _singleSourceVoice.Stop();
                }
                _isPlaying = false;
                _isFading = false;
            }
            else
            {
                throw new Exception("Cannot stop overlapped audio.");
            }
        }
    }
}
