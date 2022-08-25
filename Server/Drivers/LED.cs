namespace WS2812BServer.Drivers
{
	public class LED
	{
		public int Id { get; set; }

		public int Argb { get; set; }

		public LED(int id, int argb)
		{
			Id = id;
			Argb = argb;
		}
	}
}
