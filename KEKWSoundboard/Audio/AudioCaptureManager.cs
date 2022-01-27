using KEKWSoundboard.Audio.SampleProviders;
using KEKWSoundboard.Database;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KEKWSoundboard.Audio
{
    internal class AudioCaptureManager
    {
        WasapiCapture capture;
        WasapiOut output, secondaryOutput;
        BufferedWaveProvider waveStream, waveStream2;

        public AudioCaptureManager(MMDevice inDevice, MMDevice outDevice, MMDevice secondaryOutDevice, float volume)
        {
            try
            {
                // Setup the capture device
                capture = new WasapiCapture(inDevice, true, 10);
                capture.DataAvailable += Capture_DataAvailable;
                capture.RecordingStopped += Capture_RecordingStopped;
                capture.StartRecording();

                // Setup the rendering data stream
                waveStream = new BufferedWaveProvider(capture.WaveFormat);
                waveStream.DiscardOnBufferOverflow = true;
                waveStream.BufferLength = 512 * capture.WaveFormat.SampleRate;

                // Calculate the left/right mix
                float left = 0;
                float right = 0;

                switch(DatabaseManager.CaptureMixMode)
                {
                    case CaptureMixMode.LeftRight:
                        left = 1;
                        right = 1;
                        break;
                    case CaptureMixMode.Left:
                        left = 1;
                        break;
                    case CaptureMixMode.Right:
                        left = 1;
                        break;
                }

                // Load all the mixing providers
                List<ISampleProvider> providers = new List<ISampleProvider>();
                providers.Add(new WaveMixer32(waveStream.ToSampleProvider().ToMono(left, right)));
                providers.Add(new PanningSampleProvider(providers.Last()) { PanStrategy = new StereoBalanceStrategy() }.ToMono());
                providers.Add(new VolumeSampleProvider(providers.Last()) { Volume = volume });

                // Setup the rendering device
                output = new WasapiOut(outDevice, AudioClientShareMode.Shared, true, 10);
                output.PlaybackStopped += Output_PlaybackStopped;
                output.Init(providers.Last());

                if (secondaryOutDevice != null)
                {
                    // Setup the secondary rendering data stream
                    waveStream2 = new BufferedWaveProvider(capture.WaveFormat);
                    waveStream2.DiscardOnBufferOverflow = true;
                    waveStream2.BufferLength = 512 * capture.WaveFormat.SampleRate;

                    // Re-load the mixing providers
                    providers.Clear();
                    providers.Add(new WaveMixer32(waveStream2.ToSampleProvider()).ToMono(left, right));
                    providers.Add(new PanningSampleProvider(providers.Last()) { PanStrategy = new StereoBalanceStrategy() }.ToMono());
                    providers.Add(new VolumeSampleProvider(providers.Last()) { Volume = volume });

                    // Setup the secondary rendering device
                    secondaryOutput = new WasapiOut(secondaryOutDevice, AudioClientShareMode.Shared, true, 10);
                    secondaryOutput.PlaybackStopped += Output_PlaybackStopped;
                    secondaryOutput.Init(providers.Last());
                    secondaryOutput.Play();
                }

                output.Play();
            }
            catch (Exception e)
            {
                Console.Write("MIC CHAIN ERROR: " + e.Message);
            }
        }

        private void Output_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            Console.WriteLine("Mic playback stopped");
        }

        private void Capture_RecordingStopped(object? sender, NAudio.Wave.StoppedEventArgs e)
        {
            Console.WriteLine("Mic capture stopped");
        }

        private void Capture_DataAvailable(object? sender, NAudio.Wave.WaveInEventArgs e)
        {
            waveStream?.AddSamples(e.Buffer, 0, e.BytesRecorded);
            waveStream2?.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }

        public void Dispose()
        {
            output?.Stop();
            secondaryOutput?.Stop();

            output?.Dispose();
            secondaryOutput?.Dispose();

            waveStream?.ClearBuffer();
            waveStream2?.ClearBuffer();

            capture?.StopRecording();
            capture?.Dispose();
        }
    }
}
