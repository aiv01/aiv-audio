using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiv.Audio.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			foreach (string device in AudioDevice.Devices)
			{
				Console.WriteLine(device);
			}

			foreach (string device in AudioDevice.CaptureDevices)
			{
				Console.WriteLine(device);
			}


			Console.WriteLine(AudioDevice.CurrentDevice.Name);

			AudioClip clip = new AudioClip("Assets/jumping.ogg");

			AudioClip laser = new AudioClip("Assets/laser.wav");

			Console.WriteLine(clip.Channels);
			Console.WriteLine(clip.Frequency);
			Console.WriteLine(clip.Samples);
			Console.WriteLine(clip.Duration);

			AudioSource source = new AudioSource();

			source.Play(clip);

			AudioCapture microphone = new AudioCapture(22050, 1, 5f);
			AudioBuffer micBuffer = new AudioBuffer();
			microphone.Start();

			while (true)
			{
				Console.ReadLine();
				source.Play(clip);
				Console.ReadLine();
				source.Play(laser);
				Console.ReadLine();

				microphone.Read(micBuffer);
				source.Play(micBuffer);

			}
		}

	}
}
