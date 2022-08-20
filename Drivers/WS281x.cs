﻿using Native;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WS2812BServer.Drivers
{
	/// <summary>
	/// Wrapper class to controll WS281x LEDs
	/// </summary>
	public class WS281x : IDisposable
	{
		private ws2811_t _ws2811;
		private GCHandle _ws2811Handle;
		private bool _isDisposingAllowed;

		/// <summary>
		/// Initialize the wrapper
		/// </summary>
		/// <param name="settings">Settings used for initialization</param>
		public WS281x(Settings settings)
		{
			_ws2811 = new ws2811_t();
			//Pin the object in memory. Otherwies GC will probably move the object to another memory location.
			//This would cause errors because the native library has a pointer on the memory location of the object.
			_ws2811Handle = GCHandle.Alloc(_ws2811, GCHandleType.Pinned);

			_ws2811.dmanum = settings.DMAChannel;
			_ws2811.freq = settings.Frequency;
			_ws2811.channel_0 = new ws2811_channel_t();

			InitChannel(ref _ws2811, 0, settings.Channels[0]);

			Settings = settings;

			var initResult = PInvoke.ws2811_init(ref _ws2811);
			if (initResult != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(initResult);
				throw new Exception($"Error while initializing.\nError code: {initResult}\nMessage: {returnMessage}");
			}

			//Disposing is only allowed if the init was successful.
			//Otherwise the native cleanup function throws an error.
			_isDisposingAllowed = true;
		}

		/// <summary>
		/// Renders the content of the channels
		/// </summary>
		public void Render()
		{
			var ledColor = Settings.Channels[0].LEDs;
			Marshal.Copy(ledColor, 0, _ws2811.channel_0.leds, ledColor.Length);

			var result = PInvoke.ws2811_render(ref _ws2811);

			if (result == ws2811_return_t.WS2811_SUCCESS) 
				return;

			var returnMessage = GetMessageForStatusCode(result);
			throw new Exception($"Error while rendering.\nError code: {result}\nMessage: {returnMessage}");
		}

		/// <summary>
		/// Sets the color of a given LED
		/// </summary>
		/// <param name="channelIndex">Channel which controls the LED</param>
		/// <param name="ledId">ID/Index of the LED</param>
		/// <param name="color">New color</param>
		public void SetLEDColors(int channelIndex, IEnumerable<LedInfo> leds)
		{
			foreach (var led in leds)
			{
				Settings.Channels[channelIndex].LEDs[led.Id] = led.Argb;
			}
		}

		/// <summary>
		/// Returns the settings which are used to initialize the component
		/// </summary>
		public Settings Settings { get; private set; }

		/// <summary>
		/// Initialize the channel propierties
		/// </summary>
		/// <param name="channelIndex">Index of the channel tu initialize</param>
		/// <param name="channelSettings">Settings for the channel</param>
		private void InitChannel(ref ws2811_t ws2812, int id, Channel channelSettings)
		{
			if (id == 0)
			{
				ws2812.channel_0 = new ws2811_channel_t
				{
					count = channelSettings.LEDs.Length,
					gpionum = channelSettings.GPIOPin,
					brightness = channelSettings.Brightness,
					invert = Convert.ToInt32(channelSettings.Invert)
				};

				if (channelSettings.StripType != StripType.Unknown)
				{
					//Strip type is set by the native assembly if not explicitly set.
					//This type defines the ordering of the colors e. g. RGB or GRB, ...
					ws2812.channel_0.strip_type = (int) channelSettings.StripType;
				}
			}
			else if(id == 1)
			{
				ws2812.channel_1 = new ws2811_channel_t
				{
					count = channelSettings.LEDs.Length,
					gpionum = channelSettings.GPIOPin,
					brightness = channelSettings.Brightness,
					invert = Convert.ToInt32(channelSettings.Invert)
				};

				if (channelSettings.StripType != StripType.Unknown)
				{
					//Strip type is set by the native assembly if not explicitly set.
					//This type defines the ordering of the colors e. g. RGB or GRB, ...
					ws2812.channel_1.strip_type = (int)channelSettings.StripType;
				}
			}
		}

		/// <summary>
		/// Returns the error message for the given status code
		/// </summary>
		/// <param name="statusCode">Status code to resolve</param>
		/// <returns></returns>
		private string GetMessageForStatusCode(ws2811_return_t statusCode)
		{
			var strPointer = PInvoke.ws2811_get_return_t_str((int)statusCode);
			return Marshal.PtrToStringAuto(strPointer);
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				if (_isDisposingAllowed)
				{
					PInvoke.ws2811_fini(ref _ws2811);
					_ws2811Handle.Free();

					_isDisposingAllowed = false;
				}

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~WS281x()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}