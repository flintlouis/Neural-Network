namespace Flian
{
	public struct Position
	{
		public Position(int x, int y, int z)
		{
			this.X = (short)x;
			this.Y = (short)y;
			this.Z = (short)z;
		}

		public readonly short X;
		public readonly short Y;
		public readonly short Z;
	}
}