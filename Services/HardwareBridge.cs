using System.Drawing;
using WS2812BServer.Drivers;

namespace WS2812BServer.Services;

public interface ILedDriver : IDisposable
{
	void SetColor(IEnumerable<LedInfo> leds);
}

public class HardwareBridge : ILedDriver
{
	private readonly WS281x _ws2812;

	private const int CHANNEL = 0;

	public HardwareBridge()
	{
		var settings = Settings.CreateDefaultSettings();
		settings.Channels[CHANNEL] = new Channel(300, 18, 190, false, StripType.WS2812_STRIP);
		_ws2812 = new WS281x(settings);
	}

	public void SetColor(IEnumerable<LedInfo> leds)
	{
		_ws2812.SetLEDColors(CHANNEL, leds);
		_ws2812.Render();
	} 

	public void Dispose()
	{
		_ws2812.Dispose();
	}
}