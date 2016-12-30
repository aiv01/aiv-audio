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
		public short BitsPerSample
		{
			get
			{
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

		public virtual void Rewind()
		{ 
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
