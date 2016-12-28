using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVorbis;

namespace Aiv.Audio
{
    public class VorbisLoader : AudioLoader
    {
        private VorbisReader reader;

        public override void Open(string fileName)
        {
            base.Open(fileName);
            reader = new VorbisReader(this.stream, false);
            channels = reader.Channels;
            frequency = reader.SampleRate;
            samples = (int)reader.TotalSamples;
        }

        public override float[] ReadSamples(int amount)
        {
            float[] buffer = new float[amount];
            // ReadSamples could return less data than required for various reasons
            int count = this.reader.ReadSamples(buffer, 0, buffer.Length);
            if (count == buffer.Length)
                return buffer;
            // prepare a new buffer
            float[] newBuffer = new float[count];
            buffer.CopyTo(newBuffer, 0);
            return newBuffer;
        }
    }
}
