using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Aiv.Audio.Example.Streamer
{
    class Program
    {
        static void Main(string[] args)
        {
            AudioClip clip = new AudioClip("Assets/test_wikipedia.ogg");

            AudioSource streamer = new AudioSource(5);

            float requestedTime = 1.0f;

            int amount = (int)(clip.Frequency * requestedTime * clip.Channels);
            float[] data = null;
            while (true)
            {
                if (data == null)
                {
                    data = clip.ReadSamples32(amount);
                }
                if (data.Length == 0)
                {
                    Console.WriteLine("END OF THE STREAM");
                    clip.Rewind();
                    data = null;
                    continue;
                }

                if (!streamer.Enqueue(data, clip.Frequency, clip.Channels))
                {
                    Console.WriteLine("PLEASE SLOW DOWN !!!");
                }
                else
                {
                    data = null;
                }

                Thread.Sleep(900);
            }
        }
    }
}
