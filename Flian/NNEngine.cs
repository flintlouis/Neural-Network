using System;
using System.IO;

namespace Flian
{
	public class NNEngine : IEngine
	{
		private readonly NeuralNetwork network;
		private readonly Random rnd;

		public NNEngine(NeuralNetwork network)
		{
			this.network = network;
			this.rnd = new Random();
		}

		public Position Calculate(Board board, CellValue color)
		{
			if (color == CellValue.Empty)
				throw new ArgumentException(nameof(color));

			double[] input = this.ConvertToFlatArray(board);

			if (color == CellValue.White)
				this.FlipColor(input);

			double[] output = this.network.Calculate(input);

			Valuation[] bestValuations = this.GetTopThreeValuations(input, output);
			int index = this.ChooseValuation(bestValuations);

			return this.ConvertToPosition(index);
		}

		public string Id => this.network.Id;

		public void Serialize(BinaryWriter writer) => this.network.Serialize(writer);

		public NNEngine Mutate() => new NNEngine(this.network.Mutate(this.rnd, 0.50, 0.40));

		private double[] ConvertToFlatArray(Board board)
		{
			double[] input = new double[64];

			for (int x = 0; x < 4; x++)
			for (int y = 0; y < 4; y++)
			for (int z = 0; z < 4; z++)
			{
				input[x + 4 * y + 16 * z] = (int)board[new Position(x, y, z)];
			}

			return input;
		}

		private void FlipColor(double[] values)
		{
			for (int i = 0; i < 64; i++)
				values[i] = -values[i]; // Because black enum == 1 and white enum == -1
		}

		private Position ConvertToPosition(int index)
		{
			int x = index % 4;
			int y = (index / 4) % 4;
			int z = index / 16;

			return new Position(x, y, z);
		}

		private Valuation[] GetTopThreeValuations(double[] input, double[] output)
		{
			// The bestValuations is an array of the three best moves calculated by the neural network.
			Valuation[] bestValuations = { Valuation.InitialValuation, Valuation.InitialValuation, Valuation.InitialValuation };

			for (int i = 0; i < 64; i++)
			{
				if (input[i] != 0)
					continue;

				double v = output[i];

				if (v > bestValuations[0].Value)
				{
					bestValuations[2] = bestValuations[1];
					bestValuations[1] = bestValuations[0];
					bestValuations[0] = new Valuation(i, v);
				}
				else if (v > bestValuations[1].Value)
				{
					bestValuations[2] = bestValuations[1];
					bestValuations[1] = new Valuation(i, v);
				}
				else if (v > bestValuations[2].Value)
				{
					bestValuations[2] = new Valuation(i, v);
				}
			}

			return bestValuations;
		}

		private int ChooseValuation(Valuation[] bestValuations)
		{
			int maxIndex;

			if (bestValuations[0].Value > 1.3 * bestValuations[1].Value || bestValuations[1].Value > 1.3 * bestValuations[2].Value)
				maxIndex = bestValuations[0].Index;
			else
				maxIndex = bestValuations[this.rnd.Next(0, 3)].Index;

			return maxIndex;
		}

		private struct Valuation
		{
			public static readonly Valuation InitialValuation = new Valuation(0, double.MinValue);

			public Valuation(int index, double value)
			{
				this.Index = index;
				this.Value = value;
			}

			public readonly int Index;
			public readonly double Value;
		}
	}
}