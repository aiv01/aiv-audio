using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Aiv.Audio
{


    public class AudioLoader : IDisposable
    {
        protected Stream stream;
        protected bool disposed;

        protected int channels;

        public int Channels
        {
            get
            {
                return channels;
            }
        }

        protected int frequency;

        public int Frequency
        {
            get
            {
                return frequency;
            }
        }

        protected int samples;

        public int Samples
        {
            get
            {
                return samples;
            }
        }

        public virtual float[] ReadSamples(int amount)
        {
            return null;
        }

        public virtual void Open(string fileName)
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            foreach(string item in assembly.GetManifestResourceNames())
            {
                Console.WriteLine(item);
            }
            if (assembly.GetManifestResourceNames().Contains<string>(fileName))
            {
                stream = assembly.GetManifestResourceStream(fileName);

            }
            else {
                StreamReader reader = new StreamReader(fileName);
                stream = reader.BaseStream;
            }
        }
        

        public virtual void Dispose()
        {
            if (disposed)
                return;
            if (this.stream != null)
                this.stream.Close();
            disposed = true;
        }

        ~AudioLoader()
        {
            if (!disposed)
                Dispose();
        }
    }


}
