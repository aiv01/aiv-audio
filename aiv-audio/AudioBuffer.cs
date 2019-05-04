using System;
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

        public int Load(float[] data, int frequency, int channels)
        {
            return Load(data, data.Length, frequency, channels);
        }

        public int Load(float[] data, int length, int frequency, int channels)
        {
            // if there is no support for float32, encode it to classic 16bit
            if (!AudioDevice.HasFloat32)
            {
                short[] encodedData = new short[length];
                for (int i = 0; i < length; i++)
                {
                    int value = (int)Math.Floor(0.5 + data[i] * 32767.0);
                    if (value > 32767)
                        value = 32767;
                    if (value < -32768)
                        value = -32768;
                    encodedData[i] = (short)value;
                }
                AL.BufferData(this.bufferId, channels > 1 ? ALFormat.Stereo16 : ALFormat.Mono16, encodedData, length * sizeof(short), frequency);
                return length;
            }

            AL.BufferData(this.bufferId, channels > 1 ? ALFormat.StereoFloat32Ext : ALFormat.MonoFloat32Ext, data, length * sizeof(float), frequency);
            return length;
        }

        public int Load(short[] data, int frequency, int channels)
        {
            return Load(data, data.Length, frequency, channels);
        }

        public int Load(short[] data, int length, int frequency, int channels)
        {
            AL.BufferData(this.bufferId, channels > 1 ? ALFormat.Stereo16 : ALFormat.Mono16, data, length * sizeof(short), frequency);
            return length;
        }

        public int Load(byte[] data, int frequency, int channels)
        {
            return Load(data, data.Length, frequency, channels);
        }

        public int Load(byte[] data, int length, int frequency, int channels)
        {
            AL.BufferData(this.bufferId, channels > 1 ? ALFormat.Stereo8 : ALFormat.Mono8, data, length, frequency);
            return length;
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
