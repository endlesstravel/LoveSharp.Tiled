namespace Love.Tiled
{
	public interface ITmxLayer: ITmxElement
	{
		float? OffsetX { get; }
		float? OffsetY { get; }
		float Opacity { get; }
		PropertyDict Properties { get; }
		bool Visible { get; }
	}
}