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
                if (fileName.EndsWith(".ogg"))
                {
                    this.loader = new VorbisLoader();
                }
            }

            this.loader.Open(fileName);
        }

        public void Load()
        {
            if (buffer != null)
                return;

            buffer = new AudioBuffer();
            buffer.Load(loader.ReadSamples(loader.Samples), loader.Channels, loader.Frequency);
        }
    }
}
