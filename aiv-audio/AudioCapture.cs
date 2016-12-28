using System;
using OpenTK.Audio.OpenAL;

namespace Aiv.Audio
{
    public class AudioCapture
    {
        private IntPtr deviceId;
        private float[] buffer;
        private int channels;
        private int frequency;

        public int Frequency
        {
            get
            {
                return frequency;
            }
        }

        public int Channels
        {
            get
            {
                return channels;
            }
        }

        public AudioCapture(string device, int frequency, int channels, float duration)
        {
            this.frequency = frequency;
            this.channels = channels;
            int bufferSize = (int)(duration * frequency * channels);
            deviceId = Alc.CaptureOpenDevice(device, frequency, channels > 1 ? ALFormat.StereoFloat32Ext : ALFormat.MonoFloat32Ext, bufferSize);
            Console.WriteLine("Buffer size " + bufferSize);
            buffer = new float[bufferSize];
        }

        public AudioCapture(int frequency, int channels, float duration) : this(Alc.GetString(IntPtr.Zero, AlcGetString.CaptureDefaultDeviceSpecifier), frequency, channels, duration)
        {
        }

        public void Start()
        {
            Alc.CaptureStart(deviceId);
        }

        public void Stop()
        {
            Alc.CaptureStop(deviceId);
        }

        public int Read(AudioBuffer audioBuffer)
        {
            int samples = 0;
            Alc.GetInteger(deviceId, AlcGetInteger.CaptureSamples, 1, out samples);
            Alc.CaptureSamples<float>(deviceId, buffer, samples);
            audioBuffer.Load(buffer, samples, frequency, channels);
            return samples;
        }
    }
}
