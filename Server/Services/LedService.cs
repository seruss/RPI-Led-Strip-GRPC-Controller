using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Diagnostics;

namespace WS2812BServer.Services;

public class LedService : LedHandler.LedHandlerBase
{
	private readonly Empty _empty;
	private readonly ILedDriver _driver;
	private readonly ILogger<LedService> _logger;

	public LedService(ILedDriver driver, ILogger<LedService> logger)
	{
		_driver = driver;
		_empty = new();
		_logger = logger;
	}

	public override async Task<Empty> StartStream(IAsyncStreamReader<LedRequest> requestStream, ServerCallContext context)
	{
		var stopwatch = Stopwatch.StartNew();
		var counter = 0;
		while (await requestStream.MoveNext())
		{
			_driver.SetColor(requestStream.Current.Leds);
			counter++;
		}
		stopwatch.Stop();
		_logger.LogInformation($"Average fps: {counter / stopwatch.Elapsed.Seconds}");
		return _empty;
	}
}