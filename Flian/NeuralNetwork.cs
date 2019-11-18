using System;
using System.Collections.Generic;
using System.IO;

namespace Flian
{
	public class NeuralNetwork
	{
		private readonly List<Layer> layers;

		public NeuralNetwork(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			this.Id = reader.ReadString();
			this.Generation = reader.ReadInt32();
			int layerCount = reader.ReadInt32();

			this.layers = new List<Layer>(layerCount);

			while (layerCount > 0)
			{
				this.layers.Add(new Layer(reader, version));
				layerCount--;
			}
		}

		public NeuralNetwork(EngineConfig config) // Create a list of total amount of layers
		{
			this.Id = Guid.NewGuid().ToString("N");
			this.Generation = 1;

			this.layers = new List<Layer>(config.LayerCount);
			this.layers.Add(new Layer(config.InputSize, config.LayerSize)); // Creating first input layer

			for (int i = 1; i < config.LayerCount - 1; i++) // Minus 1 for the last layer
				this.layers.Add(new Layer(config.LayerSize, config.LayerSize));

			this.layers.Add(new Layer(config.LayerSize, config.OutputSize)); // Creating last hidden layer with weights to output layer
		}

		private NeuralNetwork(List<Layer> layers, int generation)
		{
			this.Id = Guid.NewGuid().ToString("N");
			this.Generation = generation;
			this.layers = layers;
		}

		public string Id { get; }

		public int Generation { get; }

		public void Randomize(Random rnd) // Randomize weights for each layer
		{
			foreach (var layer in this.layers)
				layer.Randomize(rnd);
		}

		public double[] Calculate(double[] input) // Calculate the output layer and return all node values in an array
		{
			double[] values = input;

			foreach (Layer layer in this.layers)
				values = layer.Calculate(values);

			return values;
		}

		public NeuralNetwork Mutate(Random rnd, double chance, double rate) // Mutate each weight in every layer
		{
			var mutated = new List<Layer>(this.layers.Count);

			foreach (Layer layer in this.layers)
				mutated.Add(layer.Mutate(rnd, chance, rate));

			return new NeuralNetwork(mutated, this.Generation + 1);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(1); // Version
			writer.Write(this.Id);
			writer.Write(this.Generation);
			writer.Write(this.layers.Count);

			for (int i=0; i<this.layers.Count; i++)
				this.layers[i].Serialize(writer);
		}
	}

	public class EngineConfig
	{
		public int InputSize; // Input layer size
		public int OutputSize; // Output layer size
		public int LayerCount; // Total layers, input layer + 3 hidden layers
		public int LayerSize; // Hidden layers size
	}
}