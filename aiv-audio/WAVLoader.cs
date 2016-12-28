using System;

namespace Aiv.Audio
{
	public class WAVLoader : AudioLoader
	{

		public class WAVParserException : Exception
		{
			public WAVParserException(string message) : base(message) { }
		}


		public override void Open(string fileName)
		{
			base.Open(fileName);
			// specs taken from http://soundfile.sapp.org/doc/WaveFormat/
			byte[] buffer = new byte[44];
			this.stream.Read(buffer, 0, 44);
			uint chunkId = ParseBigEndian32(buffer, 0);
			if (chunkId != 0x52494646)
			{
				throw new WAVParserException("Invalid ChunkID: " + chunkId.ToString("X"));
			}
			uint wave = ParseBigEndian32(buffer, 8);
			if (wave != 0x57415645)
			{
				throw new WAVParserException("Invalid Wave Format: " + wave.ToString("X"));
			}
			uint subChunkId = ParseBigEndian32(buffer, 12);
			if (subChunkId != 0x666d7420)
			{
				throw new WAVParserException("Invalid SubChunkID: " + subChunkId.ToString("X"));
			}

			ushort audioFormat = ParseLittleEndian16(buffer, 20);
			if (audioFormat != 1)
			{
				throw new WAVParserException("Unsupported AudioFormat: " + audioFormat.ToString("X"));
			}

			channels = ParseLittleEndian16(buffer, 22);
			frequency = (int)ParseLittleEndian32(buffer, 24);
			bitsPerSample = (short)ParseLittleEndian16(buffer, 34);

			uint subChunk2Id = ParseBigEndian32(buffer, 36);
			if (subChunk2Id != 0x64617461)
			{
				throw new WAVParserException("Invalid SubChunk2ID: " + subChunk2Id.ToString("X"));
			}
			samples = (int)(ParseLittleEndian32(buffer, 40) / (bitsPerSample / 8));

		}

		public override float[] ReadSamples32(int amount)
		{
			byte[] buffer = new byte[amount * sizeof(float)];
			int count = this.stream.Read(buffer, 0, amount * sizeof(float));
			int floatCount = count / sizeof(float);
			float[] newBuffer = new float[floatCount];
			for (int i = 0; i < newBuffer.Length; i++)
			{
				newBuffer[i] = BitConverter.ToSingle(buffer, i * sizeof(float));
			}
			return newBuffer;
		}

		public override short[] ReadSamples16(int amount)
		{
			byte[] buffer = new byte[amount * sizeof(short)];
			int count = this.stream.Read(buffer, 0, amount * sizeof(short));
			int uShortCount = count / sizeof(ushort);
			short[] newBuffer = new short[uShortCount];
			for (int i = 0; i < newBuffer.Length; i++)
			{
				newBuffer[i] = BitConverter.ToInt16(buffer, i * sizeof(short));
			}
			return newBuffer;
		}

		public override byte[] ReadSamples8(int amount)
		{
			byte[] buffer = new byte[amount * sizeof(ushort)];
			int count = this.stream.Read(buffer, 0, amount);
			byte[] newBuffer = new byte[count];
			Array.Copy(buffer, newBuffer, count);
			return newBuffer;
		}

		public override void Rewind()
		{
			// move to byte 44
			this.stream.Seek(44, System.IO.SeekOrigin.Begin);
		}
	}
}
