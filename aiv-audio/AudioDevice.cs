using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;

namespace Aiv.Audio
{
    public class AudioDevice
    {

        private IntPtr deviceId;
        private OpenTK.ContextHandle contextHandle;
        private int listenerId;

        public static string[] Devices
        {
            get
            {
                return Alc.GetString(IntPtr.Zero, AlcGetStringList.AllDevicesSpecifier).ToArray<string>();
            }
        }

        public AudioDevice(string device = null)
        {
            if (device == null)
            {
                device = Devices[0];
            }
            deviceId = Alc.OpenDevice(device);
            contextHandle = Alc.CreateContext(deviceId, new int[] { });
            this.Use();
        }

        public void Use()
        {
            Alc.MakeContextCurrent(contextHandle);
        }

        ~AudioDevice()
        {
            Alc.DestroyContext(contextHandle);
            Alc.CloseDevice(deviceId);
        }

    }
}
