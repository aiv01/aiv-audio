using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;

namespace Aiv.Audio
{
    public class AudioSource
    {
        private int sourceId;

        public AudioSource()
        {
            sourceId = AL.GenSource();
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
            this.Stop();
            AudioBuffer buffer = clip.Buffer;
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
        }

        public void Pause()
        {
            AL.SourcePause(sourceId);
        }
    }
}
