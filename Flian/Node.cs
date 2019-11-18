using System;
using System.IO;
using System.Xml.Schema;

namespace Flian
{
	class Node
	{
		private readonly double[] connections;

		public Node(BinaryReader reader, int length, int version)
		{
			this.connections = new double[length];

			for (int i = 0; i < length; i++)
				this.connections[i] = reader.ReadDouble();
		}

		public Node(int length) // Create array of weights for node
		{
			this.connections = new double[length];
		}

		public void Randomize(Random rnd) 
		{
			for (int i = 0; i < this.connections.Length; i++)
				this.connections[i] = 2 * rnd.NextDouble() - 1;
		}

		public Node Mutate(Random rnd, double chance, double rate)
		{
			var mutation = new Node(this.connections.Length);

			for (int i = 0; i < this.connections.Length; i++)
			{
                if (rnd.NextDouble() <= chance) // % chance to mutate
				{
					double delta = 2 * rnd.NextDouble() - 1;
					delta *= delta * delta * rate;
					mutation.connections[i] = Math.Min(4, Math.Max(-4, this.connections[i] + delta));
                }
			}

			return mutation;
		}

		public double this[int index] => this.connections[index];

		public void Serialize(BinaryWriter writer)
		{
			for(int i=0; i < this.connections.Length; i++)
				writer.Write(this.connections[i]);
		}
	}
}