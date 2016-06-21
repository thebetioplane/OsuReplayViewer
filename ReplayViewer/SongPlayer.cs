using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

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
                return Bass.BASS_ChannelBytes2Seconds(this.stream, Bass.BASS_ChannelGetPosition(this.stream)) * 1000;
            }
        }
        private string songPath = "";
        private byte playing = 0;
        private int stream = 0;

        public SongPlayer()
        {
            BassNetRegister.Register();
        }

        public void Start(string path)
        {
            this.Stop();
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            this.songPath = path;
            this.stream = Bass.BASS_StreamCreateFile(this.songPath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            this.stream = BassFx.BASS_FX_TempoCreate(this.stream, BASSFlag.BASS_FX_TEMPO_ALGO_LINEAR);
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
                Bass.BASS_ChannelPause(this.stream);
            }
        }

        public void Play()
        {
            if (this.stream != 0)
            {
                this.playing = 1;
                Bass.BASS_ChannelPlay(this.stream, false);
            }
        }

        public void Stop()
        {
            if (this.playing != 0 && this.stream != 0)
            {
                this.playing = 0;
                Bass.BASS_StreamFree(this.stream);
                Bass.BASS_Free();
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
                Bass.BASS_ChannelSetPosition(this.stream, ms / (double)1000);
            }
        }

        public void SetPlaybackSpeed(float value)
        {
            if (this.stream != 0)
            {
                value = 100 * (value - 1);
                if(! Bass.BASS_ChannelSetAttribute(this.stream, BASSAttribute.BASS_ATTRIB_TEMPO, value))
                {
                    throw new Exception(String.Format("Could not change tempo to {0}%", value));
                }
            }
        }

        public void SetVolume(float value)
        {
            if (this.stream != 0)
            {
                if (! Bass.BASS_ChannelSetAttribute(this.stream, BASSAttribute.BASS_ATTRIB_VOL, value))
                {
                    throw new Exception(String.Format("Could not change volume to {0}%", value * 100));
                }
            }
        }
    }
}
