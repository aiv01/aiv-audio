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
            AudioDevice currentDevice = new AudioDevice();

            foreach(string DeviceName in AudioDevice.Devices)
            {
                Console.WriteLine(DeviceName);
            }

            foreach (string DeviceName in AudioDevice.CaptureDevices)
            {
                Console.WriteLine(DeviceName);
            }

            Console.WriteLine(AudioDevice.CurrentDevice.Name);

            AudioClip clip001 = new AudioClip("Assets/test_wikipedia.ogg");
            AudioClip clip002 = new AudioClip("Assets/Tremolo_picking.ogg");

            Console.WriteLine(clip001.Duration + " " + clip001.Channels + " " + clip001.Samples);
            Console.WriteLine(clip002.Duration + " " + clip002.Channels + " " + clip002.Samples);

            AudioSource streamer001 = new AudioSource(5);

            AudioSource streamer002 = new AudioSource(5);

            streamer002.Volume = 0.5f;

            float requestedTime = 1.1f;

            int amount001 = (int)(clip001.Frequency * requestedTime * clip001.Channels);
            float[] data001 = null;

            int amount002 = (int)(clip002.Frequency * requestedTime * clip002.Channels);
            float[] data002 = null;

            while (true)
            {
                Console.WriteLine(streamer001.SampleOffset + " " + streamer002.SampleOffset);
                Console.WriteLine(streamer001.ByteOffset + " " + streamer002.ByteOffset);

                if (data001 == null)
                {
                    data001 = clip001.ReadSamples32(amount001);
                }
                if (data001.Length == 0)
                {
                    Console.WriteLine("END OF THE STREAM 001");
                    clip001.Rewind();
                    data001 = null;
                }

                if (data002 == null)
                {
                    data002 = clip002.ReadSamples32(amount002);
                }
                if (data002.Length == 0)
                {
                    Console.WriteLine("END OF THE STREAM 002");
                    clip002.Rewind();
                    data002 = null;
                }

                if (data001 != null)
                {
                    if (!streamer001.Enqueue(data001, clip001.Frequency, clip001.Channels))
                    {
                        Console.WriteLine("PLEASE SLOW DOWN STREAM 002!!!");
                    }
                    else
                    {
                        data001 = null;
                    }
                }

                if (data002 != null)
                {
                    if (!streamer002.Enqueue(data002, clip002.Frequency, clip002.Channels))
                    {
                        Console.WriteLine("PLEASE SLOW DOWN STREAM 002!!!");
                    }
                    else
                    {
                        data002 = null;
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
