using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEKWSoundboard.Audio
{
    internal class AudioPlayer
    {
        WasapiOut _wavePlayer, _secondaryPlayer;
        WaveStream _waveStream, _secondaryWaveStream;
        Action<string, AudioPlayer> _onStopped;
        string _filePath;

        public AudioPlayer(string filePath, byte[] fileData, AudioFileType audioFileType, MMDevice device, MMDevice secondaryDevice, float volume, float secondaryVolume, Action<string, AudioPlayer> onStopped)
        {
            _onStopped = onStopped;
            _filePath = filePath;

            // Setup the data reader
            switch(audioFileType)
            {
                case AudioFileType.Wav:
                    _waveStream = new WaveFileReader(new MemoryStream(fileData));
                    break;
                case AudioFileType.AIFF:
                    _waveStream = new AiffFileReader(new MemoryStream(fileData));
                    break;
                case AudioFileType.Mp3:
                    _waveStream = new Mp3FileReader(new MemoryStream(fileData));
                    break;
            }            

            // Add the data reader as a provider for rendering
            List<ISampleProvider> providers = new List<ISampleProvider>();
            providers.Add(_waveStream.ToSampleProvider());
            providers.Add(new VolumeWaveProvider16(providers.Last().ToWaveProvider16())
            {
                Volume = volume
            }.ToSampleProvider());

            // Setup the audio renderer
            _wavePlayer = new WasapiOut(device, AudioClientShareMode.Shared, false, 100);
            // Initialize the audio renderer with the rendering providers, then play
            _wavePlayer.Init(providers.Last());
            _wavePlayer.PlaybackStopped += PlaybackStopped;
            _wavePlayer.Play();

            // TODO: Consider creating a custom class similar (or based on) WasapiOut that can render to multiple devices
            if (secondaryDevice != null)
            {
                switch (audioFileType)
                {
                    case AudioFileType.Wav:
                        _secondaryWaveStream = new WaveFileReader(new MemoryStream(fileData));
                        break;
                    case AudioFileType.AIFF:
                        _secondaryWaveStream = new AiffFileReader(new MemoryStream(fileData));
                        break;
                    case AudioFileType.Mp3:
                        _secondaryWaveStream = new Mp3FileReader(new MemoryStream(fileData));
                        break;
                }

                _secondaryPlayer = new WasapiOut(secondaryDevice, AudioClientShareMode.Shared, false, 100);

                // Flush the previous rendering providers and add the second data reader
                providers.Clear();
                providers.Add(_secondaryWaveStream.ToSampleProvider());
                providers.Add(new VolumeWaveProvider16(providers.Last().ToWaveProvider16())
                {
                    Volume = secondaryVolume
                }.ToSampleProvider());

                _secondaryPlayer.Init(providers.Last());
                _secondaryPlayer.Play();
            }
        }

        public void Stop()
        {
            _wavePlayer.Stop();
            _secondaryPlayer.Stop();
        }

        private void PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            _secondaryPlayer?.Dispose();
            _wavePlayer.Dispose();

            _secondaryWaveStream?.Dispose();
            _waveStream.Dispose();

            _onStopped.Invoke(_filePath, this);
        }
    }
}
