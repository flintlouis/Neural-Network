using System;
using System.Collections.Generic;
using System.Linq;

namespace Flian
{
	public class Tournament
	{
		private readonly int size;
		private readonly List<NNEngine> engines;
		private readonly EngineRepository repository;

		public Tournament(EngineRepository repository, int size)
		{
			this.size = size;
			this.repository = repository;

			this.engines = new List<NNEngine>();
			this.engines.AddRange(repository.GetTopEngines(size));
		}

		public NNEngine Train(int rounds) // Train AI
		{
			List<NNEngine> winners = null;
			int sizeSqrt = (int)Math.Floor(Math.Sqrt(this.size));

			for (int i = 0; i < rounds; i++) // Training rounds
			{
				Console.WriteLine($"    Training round {i + 1}");

				winners = this.PlayRound(sizeSqrt);

				this.repository.Store(winners[0]);
				this.engines.Clear();

				foreach (NNEngine winner in winners)
				{
					this.engines.Add(winner);

					for (int j = 0; j < sizeSqrt - 1; j++)
						this.engines.Add(winner.Mutate());
				}

				while(this.engines.Count < this.size)
					this.engines.Add(this.repository.CreateRandom());
			}

			return winners[0]; // Returns top engine
		}

		public List<NNEngine> PlayRound(int numberOfWinners) // Play engines against each other
		{
			var ews = this.engines.Select(e => new EngineWithScore(e)).ToList();
            // var ai = new BasicAI();

			CellValue result;

			for (int i = 0; i < this.size; i++)
			for (int j = 0; j < this.size; j++)
			{
				if (i == j) // Skip playing against itself and play against AI
                {
					/*
                    result = this.PlayGame(ews[i].Engine, ai);

					if (result == CellValue.White) // Add score to engine if he won
						ews[i].Score += 18;
					*/
					continue;
                }

				result = this.PlayGame(ews[i].Engine, ews[j].Engine);

				if (result == CellValue.White) // Add score to engine who won
					ews[i].Score += 3;
				else if (result == CellValue.Black)
					ews[j].Score += 3;
			}

			for (int i = 0; i < 3 * this.size; i++)
			{
				var engine = this.repository.CreateRandom();

				for (int j = 0; j < this.size; j++)
				{
					result = this.PlayGame(ews[j].Engine, engine);

					if (result == CellValue.White) 
						ews[j].Score += 1;
					else if (result == CellValue.Black)
						ews[j].Score -= 1;

					result = this.PlayGame(engine, ews[j].Engine);

					if (result == CellValue.Black)
						ews[j].Score += 1;
					else if (result == CellValue.White)
						ews[j].Score -= 1;
				}
			}

			return ews.OrderByDescending(e => e.Score).Take(numberOfWinners).Select(e => e.Engine).ToList();
		}

		public CellValue PlayGame(IEngine white, IEngine black)
		{
			var game = new Game(white, black);
			return game.Play();
		}
	}

	public class EngineWithScore 
	{
		public EngineWithScore(NNEngine engine)
		{
			this.Engine = engine;
			this.Score = 0;
		}

		public NNEngine Engine { get; }
		public int Score { get; set; }
	}
}
