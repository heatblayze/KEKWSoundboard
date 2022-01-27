using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEKWSoundboard.Audio.SampleProviders
{
    public enum MixerMode { Additive, Averaging }

    public class WaveMixer32 : ISampleProvider
    {
        private readonly List<ISampleProvider> _inputs, _toAdd, _toRemove;

        public MixerMode Mode { get; set; }
        public WaveFormat WaveFormat { get; set; }


        public WaveMixer32(WaveFormat format)
        {
            _inputs = new List<ISampleProvider>();
            _toAdd = new List<ISampleProvider>();
            _toRemove = new List<ISampleProvider>();

            Mode = MixerMode.Additive;

            WaveFormat = format;
        }

        public WaveMixer32(ISampleProvider firstInput) : this(firstInput.WaveFormat)
        {
            _inputs.Add(firstInput);
        }

        public void AddInput(ISampleProvider waveProvider)
        {
            if (!waveProvider.WaveFormat.Equals(WaveFormat))
                throw new ArgumentException("All incoming channels must have the same format", "waveProvider.WaveFormat");

            _toAdd.Add(waveProvider);
        }

        public void AddInputs(IEnumerable<ISampleProvider> inputs)
        {
            inputs.ToList().ForEach(AddInput);
        }

        public void RemoveInput(ISampleProvider waveProvider)
        {
            _toRemove.Add(waveProvider);
        }

        public int InputCount => _inputs.Count;

        public int Read(float[] buffer, int offset, int count)
        {
            lock (_inputs)
            {
                if (_toAdd.Count != 0)
                {
                    _toAdd.ForEach(input => _inputs.Add(input));
                    _toAdd.Clear();
                }
                if (_toRemove.Count != 0)
                {
                    _toRemove.ForEach(input => _inputs.Remove(input));
                    _toRemove.Clear();
                }

                for (var i = 0; i < count; ++i)
                    buffer[offset + i] = 0;

                var readBuffer = new float[count];

                foreach (var input in _inputs)
                {
                    input.Read(readBuffer, 0, count);

                    for (var i = 0; i < count; ++i)
                        buffer[offset + i] += readBuffer[i];
                }

                if (Mode == MixerMode.Averaging && _inputs.Count != 0)
                    for (var i = 0; i < count; ++i)
                        buffer[offset + i] /= _inputs.Count;
            }
            return count;
        }
    }
}
