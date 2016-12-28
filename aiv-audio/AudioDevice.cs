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

		public static string[] Devices
		{
			get
			{
				return Alc.GetString(IntPtr.Zero, AlcGetStringList.AllDevicesSpecifier).ToArray<string>();
			}
		}

		private static AudioDevice currentDevice;
		public static AudioDevice CurrentDevice
		{
			get
			{
				if (currentDevice == null) {
					UseDefault();
				}
				return currentDevice;
			}
		}

		public static void UseDefault()
		{
			currentDevice = new AudioDevice();
		}

		public string Name
		{
			get
			{
				return Alc.GetString(this.deviceId, AlcGetString.DeviceSpecifier);
			}
		}

		public AudioDevice(string device = null)
		{
			if (device == null)
			{
				device = Alc.GetString(IntPtr.Zero, AlcGetString.DefaultDeviceSpecifier);
			}

			deviceId = Alc.OpenDevice(device);
			contextHandle = Alc.CreateContext(deviceId, new int[] { });

			this.Use();
		}

		public void Use()
		{
			Alc.MakeContextCurrent(contextHandle);
			currentDevice = this;
		}

		~AudioDevice()
		{
			Alc.DestroyContext(contextHandle);
			Alc.CloseDevice(deviceId);
		}

	}
}
