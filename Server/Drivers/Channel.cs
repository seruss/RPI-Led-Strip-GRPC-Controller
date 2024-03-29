﻿using System.Collections.ObjectModel;

namespace WS2812BServer.Drivers
{
	/// <summary>
	/// Represents the channel which holds the LEDs
	/// </summary>
	public class Channel
	{
		public Channel() : this(0, 0)
		{
		}

		public Channel(int ledCount, int gpioPin) : this(ledCount, gpioPin, 255, false, StripType.Unknown)
		{
		}

		public Channel(int ledCount, int gpioPin, byte brightness, bool invert, StripType stripType)
		{
			GPIOPin = gpioPin;
			Invert = invert;
			Brightness = brightness;
			StripType = stripType;
			LEDs = new int[ledCount];
		}

		/// <summary>
		/// Returns the GPIO pin which is connected to the LED strip
		/// </summary>
		public int GPIOPin { get; }

		/// <summary>
		/// Returns a value which indicates if the signal needs to be inverted.
		/// Set to true to invert the signal (when using NPN transistor level shift).
		/// </summary>
		public bool Invert { get; }

		/// <summary>
		/// Gets or sets the brightness of the LEDs
		/// 0 = darkes, 255 = brightest
		/// </summary>
		public byte Brightness { get; set; }

		/// <summary>
		/// Returns the type of the channel.
		/// The type defines the ordering of the colors.
		/// </summary>
		public StripType StripType { get; }

		/// <summary>
		/// Returns all LEDs on this channel
		/// </summary>
		public int[] LEDs { get; }
	}
}