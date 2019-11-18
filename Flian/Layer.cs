using System;
using System.Collections.Generic;
using System.IO;

namespace Flian
{
	public class Layer
	{
		private readonly List<Node> nodes;
        private readonly double[] bias;
		private readonly int inputSize;
		private readonly int outputSize;

		public Layer(BinaryReader reader, int version)
		{
			this.inputSize = reader.ReadInt32();
			this.outputSize = reader.ReadInt32();

			this.bias = new double[this.outputSize];

			for (int i = 0; i < this.outputSize; i++)
				this.bias[i] = reader.ReadDouble();

			this.nodes = new List<Node>(this.inputSize);

			for (int i=0; i<this.inputSize; i++)
				this.nodes.Add(new Node(reader, this.outputSize, version));
		}

		public Layer(int layerSize, int nextLayerSize) // Create list of nodes
		{
			this.inputSize = layerSize;
			this.outputSize = nextLayerSize;

            this.bias = new double[nextLayerSize]; // Holds the bias of the output layer
			this.nodes = new List<Node>(layerSize);

			for (int i = 0; i < layerSize; i++)
				this.nodes.Add(new Node(nextLayerSize));
		}

		private Layer(int layerSize, int nextLayerSize, List<Node> nodes, double[] bias)
		{
			this.inputSize = layerSize;
			this.outputSize = nextLayerSize;

            this.bias = bias;
			this.nodes = nodes;
		}

		public double[] Calculate(double[] input) // Calculate node value per layer
		{
			if (this.inputSize != input.Length)
				throw new ArgumentException(nameof(input));

			double[] output = new double[this.outputSize];

			for (int i = 0; i < this.inputSize; i++)
			{
				for (int j = 0; j < this.outputSize; j++)
				{
                    output[j] += input[i] * this.nodes[i][j];
				}
			}

			for (int i = 0; i < output.Length; i++)
			{
				output[i] = ActivationFunction(output[i] + bias[i]);
			}

			return output;
		}

		public void Randomize(Random rnd) // Randomize weights and bias
		{
            for (int i = 0; i < this.outputSize; i++)
                this.bias[i] = (double)rnd.Next(-3, 3) / 10; // Randomize bias between -0.3 - 0.3

            foreach (Node node in this.nodes)
				node.Randomize(rnd);
		}

		public Layer Mutate(Random rnd, double chance, double rate) // Mutate weights and bias
		{
			var mutatedNodes = new List<Node>(this.inputSize);
            double[] mutatedBias = new double[this.outputSize];

            for (int i = 0; i < this.bias.Length; i++)
            {

				if (rnd.NextDouble() <= chance)
                {
					double delta = 2 * rnd.NextDouble() - 1;
					delta *= delta * delta * rate;
					mutatedBias[i] = Math.Min(Math.Max(this.bias[i] + delta, -0.5), 0.5);
				}
			}

			foreach (Node node in this.nodes)
				mutatedNodes.Add(node.Mutate(rnd, chance, rate));

			return new Layer(this.inputSize, this.outputSize, mutatedNodes, mutatedBias);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(this.inputSize);
			writer.Write(this.outputSize);

			for (int i = 0; i < this.outputSize; i++)
				writer.Write(this.bias[i]);

			for (int i = 0; i < this.inputSize; i++)
				this.nodes[i].Serialize(writer);
		}

		private static double ActivationFunction(double x) // Sigmoid calculation
		{
			return (x/20) / (1 + Math.Abs(x/20));
			// return 1 / (1 + Math.Exp(-input)); // Sigmoid, between 0 and 1
			//return Math.Max(0, input);
		}
	}
}