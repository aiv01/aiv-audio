using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;

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

            AudioDevice playerEar = new AudioDevice();


            Console.WriteLine(AudioDevice.CurrentDevice.Name);

            AudioClip clip = new AudioClip("Assets/jumping.ogg");

            AudioClip laser = new AudioClip("Assets/laser.wav");

            AudioClip backgroundMusic = new AudioClip("Assets/test_wikipedia_mono.ogg");

            Console.WriteLine(clip.Channels);
            Console.WriteLine(clip.Frequency);
            Console.WriteLine(clip.Samples);
            Console.WriteLine(clip.Duration);

            AudioSource source = new AudioSource();

            source.Play(clip);

            AudioCapture microphone = new AudioCapture(22050, 1, 5f);
            AudioBuffer micBuffer = new AudioBuffer();
            microphone.Start();

            AudioSource background = new AudioSource();


            Window window = new Window(1024, 576, "Aiv.Audio Example");

           /* background.Position = new OpenTK.Vector3(window.Width / 2, window.Height / 2, 0);
            background.ReferenceDistance = 50;
            background.MaxDistance = 100;
            background.RolloffFactor = 1f;*/

            Sprite sprite = new Sprite(100, 100);

            while (window.opened)
            {
                background.Stream(backgroundMusic, window.deltaTime);

                if (window.GetKey(KeyCode.Space))
                    source.Play(clip);

                if (window.GetKey(KeyCode.Return))
                    source.Play(laser);

                if (window.GetKey(KeyCode.ShiftRight))
                {
                    microphone.Read(micBuffer);
                    source.Play(micBuffer);
                }

                if (window.GetKey(KeyCode.Right))
                    sprite.position.X += 100 * window.deltaTime;

                if (window.GetKey(KeyCode.Left))
                    sprite.position.X -= 100 * window.deltaTime;

                if (window.GetKey(KeyCode.Up))
                    sprite.position.Y -= 100 * window.deltaTime;

                if (window.GetKey(KeyCode.Down))
                    sprite.position.Y += 100 * window.deltaTime;

                //playerEar.Position = new OpenTK.Vector3(sprite.position.X, sprite.position.Y, 0);
                //source.Position = playerEar.Position;

                sprite.DrawSolidColor(1f, 0, 0);

                window.Update();
            }
        }

    }
}
