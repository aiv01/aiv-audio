using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

namespace Aiv.Audio.Tests
{
	[TestFixture]
	public class AudioLoaderTest
	{
		[OneTimeSetUp]
		public void ChangeWorkingDirectory()
		{
			string currentFile = typeof(AudioLoaderTest).Assembly.Location;
			Directory.SetCurrentDirectory(Directory.GetParent(currentFile).FullName);
        }

        [Test]
        public void TestVorbisLoader()
        {
            AudioClip clip = new AudioClip("Assets/jumping.ogg", new VorbisLoader());
            Assert.That(clip.Frequency, Is.EqualTo(44100));
        }
    }
}
