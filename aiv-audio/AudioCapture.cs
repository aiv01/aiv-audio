using System;
using OpenTK.Audio.OpenAL;

namespace Aiv.Audio
{
	public class AudioCapture
	{
		private IntPtr deviceId;
		private short[] buffer;
		private int channels;
		private int frequency;
		private IntPtr bufferPtr;

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

		unsafe public AudioCapture(string device, int frequency, int channels, float duration)
		{
			this.frequency = frequency;
			this.channels = channels;
			int bufferSize = (int)(duration * frequency * channels);
			deviceId = Alc.CaptureOpenDevice(device, frequency, channels > 1 ? ALFormat.Stereo16 : ALFormat.Mono16, bufferSize);
			buffer = new short[bufferSize];

			fixed (short* ptr = buffer)
			{
				bufferPtr = (IntPtr)ptr;
			}

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

		public int AvailableSamples {
			get {
				int samples = 0;
				Alc.GetInteger(deviceId, AlcGetInteger.CaptureSamples, 1, out samples);
				return samples;
			}
		}

		public int Read(AudioBuffer audioBuffer)
		{
			int samples = AvailableSamples;
			Alc.CaptureSamples(deviceId, bufferPtr, samples);
			audioBuffer.Load(buffer, samples, frequency, channels);
			return samples;
		}
	}
}
