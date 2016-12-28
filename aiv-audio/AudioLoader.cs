using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Aiv.Audio
{


	public class AudioLoader : IDisposable
	{
		protected Stream stream;
		protected bool disposed;

		protected int channels;

		public int Channels
		{
			get
			{
				return channels;
			}
		}

		protected int frequency;

		public int Frequency
		{
			get
			{
				return frequency;
			}
		}

		protected int samples;

		public int Samples
		{
			get
			{
				return samples;
			}
		}

		protected short bitsPerSample;
		public short BitsPerSample {
			get {
				return bitsPerSample;
			}
		}

		public virtual float[] ReadSamples32(int amount)
		{
			return null;
		}

		public virtual short[] ReadSamples16(int amount)
		{
			return null;
		}

		public virtual byte[] ReadSamples8(int amount)
		{
			return null;
		}

		protected uint ParseBigEndian32(byte[] buffer, int offset)
		{
			byte[] formatBuffer = new byte[4];
			formatBuffer[0] = buffer[offset + 3];
			formatBuffer[1] = buffer[offset + 2];
			formatBuffer[2] = buffer[offset + 1];
			formatBuffer[3] = buffer[offset];
			return BitConverter.ToUInt32(formatBuffer, 0);
		}

		protected ushort ParseBigEndian16(byte[] buffer, int offset)
		{
			byte[] formatBuffer = new byte[2];
			formatBuffer[0] = buffer[offset + 1];
			formatBuffer[1] = buffer[offset];
			return BitConverter.ToUInt16(formatBuffer, 0);
		}

		protected uint ParseLittleEndian32(byte[] buffer, int offset)
		{
			return BitConverter.ToUInt32(buffer, offset);
		}

		protected ushort ParseLittleEndian16(byte[] buffer, int offset)
		{
			return BitConverter.ToUInt16(buffer, offset);
		}

		public virtual void Open(string fileName)
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			// assembly could be null when running the test suite
			if (assembly != null && assembly.GetManifestResourceNames().Contains<string>(fileName))
			{
				stream = assembly.GetManifestResourceStream(fileName);

			}
			else {
				StreamReader reader = new StreamReader(fileName);
				stream = reader.BaseStream;
			}
		}


		public virtual void Dispose()
		{
			if (disposed)
				return;
			if (this.stream != null)
				this.stream.Close();
			disposed = true;
		}

		~AudioLoader()
		{
			if (!disposed)
				Dispose();
		}
	}


}
