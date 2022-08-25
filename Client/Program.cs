using System.Drawing;
using Grpc.Net.Client;
using WS2812BServer;

using var channel = GrpcChannel.ForAddress("http://192.168.1.217:2137");

var client = new LedHandler.LedHandlerClient(channel);

using var stream = client.StartStream();

var rnd = new Random();

var color = GetRandomColor(rnd);

var request = new LedRequest
{
    Leds = { Enumerable.Range(0, 300).Select(i => new LedInfo { Argb = color, Id = i }) }
};

while (Console.ReadKey(true).Key != ConsoleKey.Escape)
{
    if (Console.ReadKey().Key == ConsoleKey.Spacebar)
    {
        color = GetRandomColor(rnd);
        foreach (var led in request.Leds)
        {
            led.Argb = color;
        }
        await stream.RequestStream.WriteAsync(request);
    }
}

foreach (var led in request.Leds)
{
    led.Argb = Color.Black.ToArgb();
}
await stream.RequestStream.WriteAsync(request);

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

int GetRandomColor(Random rnd)
{
    return Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)).ToArgb();

}