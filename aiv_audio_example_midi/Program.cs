using System;
using Aiv.Audio;
using Aiv.Fast2D;

namespace Aiv.Audio.Example.Midi
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			AudioDevice device = new AudioDevice();

			AudioSource channel0 = new AudioSource();

			AudioClip guitar = new AudioClip("Assets/korg_m3r_rock.wav");

			AudioSequence ozzy = new AudioSequence("Assets/ozzy_osbourne_crazy_train.mid");

			Window window = new Window(1024, 576, "Aiv.Audio.Example.Midi");

			while (window.opened)
			{
				if (window.GetKey(KeyCode.Space))
					channel0.Play(guitar);
				window.Update();
			}
		}
	}
}
