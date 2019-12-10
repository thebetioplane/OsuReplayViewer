using System;
using ManagedBass;

namespace ReplayViewer
{
    public class SongPlayer
    {
        public string SongPath
        {
            get
            {
                return this.songPath;
            }
        }
        public byte Playing
        {
            get
            {
                return this.playing;
            }
        }
        public double SongTime
        {
            get
            {
                if (this.stream == 0)
                {
                    return 0;
                }
                return Bass.ChannelBytes2Seconds(this.stream, Bass.ChannelGetPosition(this.stream) * 1000);
            }
        }
        private string songPath = "";
        private byte playing = 0;
        private int stream = 0;

        public SongPlayer()
        {
        }

        public void Start(string path)
        {
            this.Stop();
            Bass.Init(-1, 44100, DeviceInitFlags.Default, IntPtr.Zero);
            this.songPath = path;
            this.stream = Bass.CreateStream(this.songPath, 0, 0, BassFlags.Decode);
            this.stream = ManagedBass.Fx.BassFx.TempoCreate(this.stream, BassFlags.FxTempoAlgorithmLinear);
            if (this.stream == 0)
            {
                throw new Exception("Audio stream could not be created.");
            }
        }

        public void Pause()
        {
            if (this.playing != 2 && this.stream != 0)
            {
                this.playing = 2;
                Bass.ChannelPause(this.stream);
            }
        }

        public void Play()
        {
            if (this.stream != 0)
            {
                this.playing = 1;
                Bass.ChannelPlay(this.stream, false);
            }
        }

        public void Stop()
        {
            if (this.playing != 0 && this.stream != 0)
            {
                this.playing = 0;
                Bass.StreamFree(this.stream);
                Bass.Free();
                this.stream = 0;
            }
        }

        public void JumpTo(long ms)
        {
            if (this.stream != 0)
            {
                if (ms < 0)
                {
                    ms = 0;
                }
                Bass.ChannelSetPosition(this.stream, Bass.ChannelSeconds2Bytes(this.stream, ms / (double)1000), PositionFlags.Bytes);
            }
        }

        public void SetPlaybackSpeed(float value)
        {
            if (this.stream != 0)
            {
                value = 100 * (value - 1);
                if (!Bass.ChannelSetAttribute(this.stream, ChannelAttribute.Tempo, value))
                {
                    throw new Exception(String.Format("Could not change tempo to {0}%", value));
                }
            }
        }

        public void SetVolume(float value)
        {
            if (this.stream != 0)
            {
                if (!Bass.ChannelSetAttribute(this.stream, ChannelAttribute.Volume, value))
                {
                    throw new Exception(String.Format("Could not change volume to {0}%", value * 100));
                }
            }
        }
    }
}
