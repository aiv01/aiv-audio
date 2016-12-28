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

		public void Load(float[] data, int frequency, int channels)
		{
			Load(data, data.Length, frequency, channels);
		}

		public void Load(float[] data, int length, int frequency, int channels)
		{
			AL.BufferData(this.bufferId, channels > 1 ? ALFormat.StereoFloat32Ext : ALFormat.MonoFloat32Ext, data, length * sizeof(float), frequency);
		}

		public void Load(short[] data, int frequency, int channels)
		{
			Load(data, data.Length, frequency, channels);
		}

		public void Load(short[] data, int length, int frequency, int channels)
		{
			AL.BufferData(this.bufferId, channels > 1 ? ALFormat.Stereo16 : ALFormat.Mono16, data, length * sizeof(short), frequency);
		}

		public void Load(byte[] data, int frequency, int channels)
		{
			Load(data, data.Length, frequency, channels);
		}

		public void Load(byte[] data, int length, int frequency, int channels)
		{
			AL.BufferData(this.bufferId, channels > 1 ? ALFormat.Stereo8 : ALFormat.Mono8, data, length, frequency);
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
