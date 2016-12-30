using System;
namespace Aiv.Audio
{
	public static class AudioUtils
	{
		public static uint ParseBigEndian32(byte[] buffer, int offset = 0)
		{
			byte[] formatBuffer = new byte[4];
			formatBuffer[0] = buffer[offset + 3];
			formatBuffer[1] = buffer[offset + 2];
			formatBuffer[2] = buffer[offset + 1];
			formatBuffer[3] = buffer[offset];
			return BitConverter.ToUInt32(formatBuffer, 0);
		}

		public static ushort ParseBigEndian16(byte[] buffer, int offset = 0)
		{
			byte[] formatBuffer = new byte[2];
			formatBuffer[0] = buffer[offset + 1];
			formatBuffer[1] = buffer[offset];
			return BitConverter.ToUInt16(formatBuffer, 0);
		}

		public static uint ParseLittleEndian32(byte[] buffer, int offset = 0)
		{
			return BitConverter.ToUInt32(buffer, offset);
		}

		public static ushort ParseLittleEndian16(byte[] buffer, int offset = 0)
		{
			return BitConverter.ToUInt16(buffer, offset);
		}

		public static int ParseVLQ(byte[] buffer, int offset, ref int index)
		{
			int value = buffer[offset];
			index++;
			if ((value & 0x80) == 0)
			{
				return value;
			}
			value &= 0x7f;
			byte c;
			do
			{
				c = buffer[++offset];
				index++;
				value = (value << 7) + (c & 0x7f);
			} while ((c & 0x80) != 0);
			return value;
		}
	}
}
