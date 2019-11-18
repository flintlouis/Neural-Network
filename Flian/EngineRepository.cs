using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flian
{
	public class EngineRepository
	{
		private readonly EngineConfig config;
		private readonly Random rnd;
		private readonly string directory;

		public EngineRepository(string directory)
		{
			this.config = new EngineConfig {
				InputSize = 64,
				OutputSize = 64,
				LayerCount = 5,
				LayerSize = 48
			};

			this.rnd = new Random();
			this.directory = directory;

			if (!Directory.Exists(this.directory))
				Directory.CreateDirectory(this.directory);
		}

		public NNEngine Load(string engineId)
		{
			string filename = Path.Combine(this.directory, engineId);

			if (!File.Exists(filename))
				throw new ArgumentException($"EngineRepository.Load() failed. No engine found with the ID \"{engineId}\".", nameof(engineId));

			using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var reader = new BinaryReader(stream))
			{
				return new NNEngine(new NeuralNetwork(reader));
			}
		}

		public NNEngine CreateRandom()
		{
			var network = new NeuralNetwork(this.config);
			network.Randomize(this.rnd);

			return new NNEngine(network);
		}

		public void Purge(int survivors)
		{
			List<NNEngine> top = this.GetTopEngines(survivors);
			var files = new HashSet<string>(top.Select(e => e.Id));

			foreach (FileInfo file in new DirectoryInfo(this.directory).GetFiles())
			{
				if (!files.Contains(file.Name))
					File.Delete(file.FullName);
			}
		}

		public List<NNEngine> GetTopEngines(int count)
		{
			var engines = new List<NNEngine>();

			foreach (string filename in Directory.GetFiles(this.directory))
				engines.Add(this.Load(filename));

			if (engines.Count < count)
			{
				while(engines.Count < count)
					engines.Add(this.CreateRandom());

				return engines;
			}

			return PlaySingleRound(engines, count);
		}

		private static List<NNEngine> PlaySingleRound(List<NNEngine> engines, int count)
		{
			var ews = engines.Select(e => new EngineWithScore(e)).ToList();

			for (int i = 0; i < ews.Count; i++)
			for (int j = 0; j < ews.Count; j++)
			{
				if (i == j)
					continue;

				var game = new Game(ews[i].Engine, ews[j].Engine);
				CellValue result = game.Play();

				if (result == CellValue.White) // Add score to engine who won
					ews[i].Score++;
				else if (result == CellValue.Black)
					ews[j].Score++;
			}

			return ews.OrderByDescending(e => e.Score).Take(count).Select(e => e.Engine).ToList();
		}

		public void Store(NNEngine engine)
		{
			string filename = Path.Combine(this.directory, engine.Id);

			using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new BinaryWriter(stream))
			{
				engine.Serialize(writer);
			}
		}
	}
}