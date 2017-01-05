using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using OpenTK;

namespace Aiv.Audio
{
    public class AudioSource : IDisposable
    {
        private int sourceId;

        private float streamAccumulator;

        private AudioBuffer[] streamingBuffers;
        private Dictionary<int, AudioBuffer> streamingBuffersMap;
        private int[] streamingBuffersIds;

        private Queue<int> freeBuffers;

        public Vector3 Position
        {
            get
            {
                float x, y, z;
                AL.GetSource(this.sourceId, ALSource3f.Position, out x, out y, out z);
                return new Vector3(x, y, z);
            }
            set
            {
                AL.Source(this.sourceId, ALSource3f.Position, ref value);
            }
        }


        public float ReferenceDistance
        {
            get
            {
                float distance;
                AL.GetSource(this.sourceId, ALSourcef.ReferenceDistance, out distance);
                return distance;
            }
            set
            {
                AL.Source(this.sourceId, ALSourcef.ReferenceDistance, value);
            }
        }

        public float MaxDistance
        {
            get
            {
                float distance;
                AL.GetSource(this.sourceId, ALSourcef.MaxDistance, out distance);
                return distance;
            }
            set
            {
                AL.Source(this.sourceId, ALSourcef.MaxDistance, value);
            }
        }

        public float RolloffFactor
        {
            get
            {
                float rolloff;
                AL.GetSource(this.sourceId, ALSourcef.RolloffFactor, out rolloff);
                return rolloff;
            }
            set
            {
                AL.Source(this.sourceId, ALSourcef.RolloffFactor, value);
            }
        }

        private int numberOfStreamingBuffers;

        public AudioSource(int numberOfStreamingBuffers = 3)
        {
            if (AudioDevice.CurrentDevice == null)
            {
                AudioDevice.UseDefault();
            }
            this.numberOfStreamingBuffers = numberOfStreamingBuffers;
            sourceId = AL.GenSource();
            // set the audiosource in a good default state
            this.Stop();
        }

        public bool IsPlaying
        {
            get
            {
                return AL.GetSourceState(this.sourceId) == ALSourceState.Playing;
            }
        }

        public void Play(AudioClip clip, bool loop = false)
        {
            this.Play(clip.Buffer, loop);
        }

        public void Play(AudioBuffer buffer, bool loop = false)
        {
            this.Stop();
            AL.Source(sourceId, ALSourcei.Buffer, buffer.Id);
            AL.Source(sourceId, ALSourceb.Looping, loop);
            this.Resume();
        }

        public void Resume()
        {
            AL.SourcePlay(sourceId);
        }

        public void Stop()
        {
            AL.SourceStop(sourceId);
            // cleanup buffer pointer, just for safety or broken implementations
            AL.Source(this.sourceId, ALSourcei.Buffer, 0);
            // set internal looping to false, otherwise streaming will not work !
            AL.Source(sourceId, ALSourceb.Looping, false);
            streamAccumulator = 0;
            if (streamingBuffers != null)
            {
                freeBuffers.Clear();
                for (int i = 0; i < streamingBuffersIds.Length; i++)
                {
                    freeBuffers.Enqueue(streamingBuffersIds[i]);
                }
            }
        }

        public void Pause()
        {
            AL.SourcePause(sourceId);
        }

        private int CheckEnqueue(int length)
        {
            // do not waste cycles on empty data
            if (length == 0)
                return -1;
            CheckStreaming();
            int processed = 0;
            AL.GetSource(this.sourceId, ALGetSourcei.BuffersProcessed, out processed);
            while (processed > 0)
            {
                // unqueue buffer
                int bufferId = AL.SourceUnqueueBuffer(this.sourceId);
                if (bufferId > -1)
                {
                    freeBuffers.Enqueue(bufferId);
                }
                processed--;
            }
            if (freeBuffers.Count > 0)
                return freeBuffers.Dequeue();
            return -1;
        }

        private void EnqueueAndPlay(int bufferId)
        {
            // requeue buffer and eventually play it
            AL.SourceQueueBuffer(this.sourceId, bufferId);
            if (!IsPlaying)
                AL.SourcePlay(this.sourceId);
        }

        public bool Enqueue(byte[] data, int frequency, int channels)
        {
            int bufferId = CheckEnqueue(data.Length);
            if (bufferId < 0)
                return false;

            streamingBuffersMap[bufferId].Load(data, frequency, channels);
            EnqueueAndPlay(bufferId);
            return true;
        }

        public bool Enqueue(float[] data, int frequency, int channels)
        {
            int bufferId = CheckEnqueue(data.Length);
            if (bufferId < 0)
                return false;

            streamingBuffersMap[bufferId].Load(data, frequency, channels);
            EnqueueAndPlay(bufferId);
            return true;
        }

        public bool Enqueue(short[] data, int frequency, int channels)
        {
            int bufferId = CheckEnqueue(data.Length);
            if (bufferId < 0)
                return false;

            streamingBuffersMap[bufferId].Load(data, frequency, channels);
            EnqueueAndPlay(bufferId);
            return true;
        }

        private void CheckStreaming()
        {
            // is it the first round ?
            if (streamingBuffers == null)
            {
                streamingBuffers = new AudioBuffer[numberOfStreamingBuffers];
                streamingBuffersMap = new Dictionary<int, AudioBuffer>();
                streamingBuffersIds = new int[numberOfStreamingBuffers];
                freeBuffers = new Queue<int>();
                for (int i = 0; i < streamingBuffers.Length; i++)
                {
                    streamingBuffers[i] = new AudioBuffer();
                    int streamingBufferId = streamingBuffers[i].Id;
                    streamingBuffersMap[streamingBufferId] = streamingBuffers[i];
                    freeBuffers.Enqueue(streamingBufferId);
                }
                streamingBuffersIds = streamingBuffersMap.Keys.ToArray();
                this.Stop();
            }
        }

        public void Stream(AudioClip clip, float deltaTime, bool loop = true)
        {
            CheckStreaming();
            // do we need to restart reading ?
            if (streamAccumulator == 0)
            {

                // stop playing (will reset the buffers too)
                this.Stop();

                int i;
                for (i = 0; i < streamingBuffers.Length; i++)
                {
                    // stream 1.5 seconds
                    int amount = clip.LoadToBuffer(streamingBuffers[i], (clip.Channels * clip.Frequency) / 2);
                    if (amount == 0)
                    {
                        if (!loop)
                            break;
                        clip.Rewind();
                        amount = clip.LoadToBuffer(streamingBuffers[i], (clip.Channels * clip.Frequency) / 2);
                    }
                }
                // enqueue buffers
                AL.SourceQueueBuffers(this.sourceId, i, streamingBuffersIds);
                AL.SourcePlay(this.sourceId);
            }
            streamAccumulator += deltaTime;
            if (streamAccumulator < 1f / 30)
                return;

            // how many buffers are already processed ?
            int processed = 0;
            AL.GetSource(this.sourceId, ALGetSourcei.BuffersProcessed, out processed);

            while (processed > 0)
            {
                // unqueue buffer
                int bufferId = AL.SourceUnqueueBuffer(this.sourceId);
                int amount = clip.LoadToBuffer(streamingBuffersMap[bufferId], (clip.Channels * clip.Frequency) / 2);
                // end of the stream
                if (amount == 0)
                {
                    if (!loop)
                    {
                        this.Stop();
                        return;
                    }
                    clip.Rewind();
                    amount = clip.LoadToBuffer(streamingBuffersMap[bufferId], (clip.Channels * clip.Frequency) / 2);
                }
                AL.SourceQueueBuffer(this.sourceId, bufferId);
                processed--;
            }

        }

        private bool disposed;
        ~AudioSource()
        {
            if (!this.disposed)
                this.Dispose();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            this.Stop();
            if (streamingBuffersIds != null)
            {
                for (int i = 0; i < streamingBuffersIds.Length; i++)
                {
                    AL.DeleteBuffer(streamingBuffersIds[i]);
                }
            }
            if (this.sourceId > -1)
                AL.DeleteSource(this.sourceId);
            disposed = true;
        }
    }
}
