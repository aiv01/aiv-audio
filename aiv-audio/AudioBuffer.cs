using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;

namespace Aiv.Audio
{
    public class AudioBuffer : IDisposable
    {
        private int bufferId;
        private bool disposed;

        public int Id
        {
            get
            {
                return bufferId;
            }
        }

        public AudioBuffer()
        {
            bufferId = AL.GenBuffer();
        }

        public void Load(float[] data, int channels, int frequency)
        {
            AL.BufferData(this.bufferId, channels > 1 ? ALFormat.StereoFloat32Ext : ALFormat.MonoFloat32Ext, data, data.Length * sizeof(float), frequency);
        }

        public void Dispose()
        {
            if (disposed)
                return;
            if (this.bufferId > -1)
                AL.DeleteBuffer(this.bufferId);
            disposed = true;
        }

        ~AudioBuffer()
        {
            if (!disposed)
                Dispose();
        }
    }
}
