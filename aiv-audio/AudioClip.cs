using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiv.Audio
{
    public class AudioClip
    {

        private AudioBuffer buffer;

        public AudioBuffer Buffer
        {
            get
            {
                this.Load();
                return buffer;
            }
        }

        private AudioLoader loader;

        public int Channels
        {
            get
            {
                return loader.Channels;
            }
        }

        public int Frequency
        {
            get
            {
                return loader.Frequency;
            }
        }

        public int Samples
        {
            get
            {
                return loader.Samples;
            }
        }

        public float Duration
        {
            get
            {
                return (float)loader.Samples / (loader.Frequency * loader.Channels);
            }
        }

        public AudioClip(string fileName, AudioLoader loader = null)
        {
            this.loader = loader;
            // try autodetection
            if (this.loader == null)
            {
                if (fileName.EndsWith(".ogg", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    this.loader = new VorbisLoader();
                }
                else if (fileName.EndsWith(".wav", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    this.loader = new WAVLoader();
                }
            }

            this.loader.Open(fileName);
        }

        public void Load()
        {
            if (buffer != null)
                return;

            buffer = new AudioBuffer();
            LoadToBuffer(buffer, loader.Samples);
        }

        public void Rewind()
        {
            loader.Rewind();
        }

        public byte[] ReadSamples8(int amount)
        {
            return loader.ReadSamples8(amount);
        }

        public short[] ReadSamples16(int amount)
        {
            return loader.ReadSamples16(amount);
        }

        public float[] ReadSamples32(int amount)
        {
            return loader.ReadSamples32(amount);
        }

        public int LoadToBuffer(AudioBuffer buffer, int amount)
        {
            if (loader.BitsPerSample == 32)
            {
                return buffer.Load(loader.ReadSamples32(amount), loader.Frequency, loader.Channels);
            }
            else if (loader.BitsPerSample == 16)
            {
                return buffer.Load(loader.ReadSamples16(amount), loader.Frequency, loader.Channels);
            }
            else if (loader.BitsPerSample == 8)
            {
                return buffer.Load(loader.ReadSamples8(amount), loader.Frequency, loader.Channels);
            }
            return -1;
        }
    }
}
