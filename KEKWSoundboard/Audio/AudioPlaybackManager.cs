using KEKWSoundboard.Database;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEKWSoundboard.Audio
{
    public enum AudioFileType
    {
        Wav,
        AIFF,
        Mp3
    }

    internal static class AudioPlaybackManager
    {
        static Dictionary<string, List<AudioPlayer>> _playingSounds = new Dictionary<string, List<AudioPlayer>>();
        static Dictionary<string, (byte[] data, AudioFileType type)> _soundsData = new Dictionary<string, (byte[] data, AudioFileType type)>();

        public static void PlaySound(string fileName, float volume)
        {
            var path = Path.GetFullPath(fileName);
            AudioPlayer player = null;
            if (_soundsData.ContainsKey(path))
            {
                // Sound is still active, don't use more disk bandwith
                // Fetch the existing sound data
                var soundData = _soundsData[path];

                // Create and start a new audio player
                player = new AudioPlayer(path, soundData.data, soundData.type,
                    DatabaseManager.PrimaryRenderDevice, DatabaseManager.SecondaryRenderDevice,
                    volume * DatabaseManager.PrimaryRenderDeviceVolume, volume * DatabaseManager.SecondaryRenderDeviceVolume,
                    SoundCompleted);
            }
            else
            {
                // Read the file data
                var rawData = File.ReadAllBytes(path);
                var ext = Path.GetExtension(path);

                AudioFileType type;

                switch(ext.ToLower())
                {
                    default:
                    case ".wav":
                        type = AudioFileType.Wav;
                        break;
                    case ".aiff":
                        type = AudioFileType.AIFF;
                        break;
                    case ".mp3":
                        type = AudioFileType.Mp3;
                        break;
                }

                // Create and start a new audio player
                player = new AudioPlayer(path, rawData, type,
                    DatabaseManager.PrimaryRenderDevice, DatabaseManager.SecondaryRenderDevice,
                    volume * DatabaseManager.PrimaryRenderDeviceVolume, volume * DatabaseManager.SecondaryRenderDeviceVolume,
                    SoundCompleted);

                // Initialize the dictionaries that store the sound data and the active sounds being played
                _soundsData.Add(path, (rawData, type));
                _playingSounds.Add(path, new List<AudioPlayer>());
            }

            // Register that a new sound is being played
            _playingSounds[path].Add(player);
        }

        public static void StopSound(string fileName)
        {
            var path = Path.GetFullPath(fileName);

            foreach (var sound in _playingSounds[path])
            {
                sound.Stop();
            }
        }

        static void SoundCompleted(string path, AudioPlayer audioPlayer)
        {
            _playingSounds[path].Remove(audioPlayer);

            if (_playingSounds[path].Count <= 0)
            {
                FlushSound(path);
            }
        }

        static void FlushSound(string path)
        {
            // All sounds have stopped playing, flush memory
            _playingSounds.Remove(path);
            _soundsData.Remove(path);
        }
    }
}
