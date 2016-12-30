using System;
using System.IO;
using System.Text;

namespace Aiv.Audio
{
	public class AudioSequence
	{

		BinaryReader reader;

		public AudioSequence(string fileName)
		{
			StreamReader stream = new StreamReader(fileName);
			reader = new BinaryReader(stream.BaseStream);
			byte[] header = new byte[4];
			reader.Read(header, 0, 4);

			string midiSignature = Encoding.ASCII.GetString(header, 0, 4);

			if (midiSignature != "MThd")
			{
				throw new Exception("not a midi file");
			}

			reader.Read(header, 0, 4);
			uint midiHeaderSize = AudioUtils.ParseBigEndian32(header);

			if (midiHeaderSize != 6)
			{
				throw new Exception("invalid midi file");
			}

			byte[] data = new byte[midiHeaderSize];
			reader.Read(data, 0, (int)midiHeaderSize);

			ushort midiFormat = AudioUtils.ParseBigEndian16(data);
			Console.WriteLine("MIDI format: " + midiFormat);

			ushort midiTracks = AudioUtils.ParseBigEndian16(data, 2);
			Console.WriteLine("MIDI tracks: " + midiTracks);

			ushort midiTime = AudioUtils.ParseBigEndian16(data, 4);
			Console.WriteLine("MIDI time: " + midiTime);

			for (int i = 0; i < midiTracks; i++)
			{
				reader.Read(header, 0, 4);
				string trackSignature = Encoding.ASCII.GetString(header, 0, 4);
				reader.Read(header, 0, 4);
				uint trackSize = AudioUtils.ParseBigEndian32(header);
				data = new byte[trackSize];
				reader.Read(data, 0, (int)trackSize);
				Console.WriteLine("track: " + trackSignature + "[" + i + "] " + trackSize + " bytes");
				if (trackSignature != "MTrk")
					continue;
				ParseTrack(data, i);
			}
		}

		private void ParseTrack(byte[] data, int trackId)
		{
			for (int i = 0; i < data.Length; i++)
			{
				int vlq = 0;
				int deltaTime = AudioUtils.ParseVLQ(data, i, ref vlq);
				Console.WriteLine("Track " + trackId + " deltaTime: " + deltaTime);
				i += vlq;
				byte midiEvent = data[i++];
				if ((midiEvent & 0xf0) == 0xf0)
				{
					vlq = 0;
					if (midiEvent == 0xff)
					{
						byte metaType = data[i++];
						Console.WriteLine("META EVENT: " + metaType.ToString("X"));
					}
					else {
						Console.WriteLine("SYSEX EVENT: " + midiEvent.ToString("X"));
					}
					int metaLength = AudioUtils.ParseVLQ(data, i, ref vlq);
					i += vlq - 1 + metaLength;
				}

				/*
				else if ((midiEvent & 0xf0) == 0x80)
				{
					Console.WriteLine("NOTE OFF " + midiEvent.ToString("X"));
					i += 4;
				}
				else if ((midiEvent & 0xf0) == 0x90)
				{
					Console.WriteLine("NOTE ON " + midiEvent.ToString("X"));
					i += 4;
				}
				else if ((midiEvent & 0xf0) == 0xb0)
				{
					Console.WriteLine("CONTROL CHANGE " + midiEvent.ToString("X"));
					i += 4;
				}
				else if ((midiEvent & 0xf0) == 0xc0)
				{
					Console.WriteLine("PROGRAM CHANGE " + midiEvent.ToString("X"));
					i += 5;

				}
				else if ((midiEvent & 0xf0) == 0xa0)
				{
					Console.WriteLine("POLYTOUCH " + midiEvent.ToString("X"));
					i += 4;
				}
				else {
					throw new Exception("Unknown event " + midiEvent.ToString("X") + " " + data[i].ToString("X") + " " + data[i+1].ToString("X") + " " + data[i + 2].ToString("X"));
				}
				*/
			}
		}
	}
}
