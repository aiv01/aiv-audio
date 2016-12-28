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
            foreach(string device in AudioDevice.Devices)
            {
                Console.WriteLine(device);
            }

           
			Console.WriteLine(AudioDevice.CurrentDevice.Name);

            AudioClip clip = new AudioClip("Assets/jumping.ogg");

            Console.WriteLine(clip.Channels);
            Console.WriteLine(clip.Frequency);
            Console.WriteLine(clip.Samples);
            Console.WriteLine(clip.Duration);

            AudioSource source = new AudioSource();

            source.Play(clip);

			while (true)
			{
				Console.ReadLine();
				source.Play(clip);
			}
        }
    }
}
