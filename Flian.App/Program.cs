using System;
using System.IO;
using System.Linq;
using NDesk.Options;

namespace Flian.App
{
	class Program
	{
        static void Main(string[] args)
		{
			string mode = "";
			string folder = "";
			string engine = "";

			var options = new OptionSet {
				{ "m|mode=", "Either train, play or human. The default is train.", str => mode = str.ToLowerInvariant() },
				{ "f|folder=", "The folder where the engines are stored.", str => folder = str },
				{ "e|engine=", "The specific engine to play against", str => engine = str }
			};

			if (args.Length == 0 || args.Any(x => x == "/?" || x == "-?" || x == "?" || x == "help" || x == "--help"))
			{
				options.WriteOptionDescriptions(Console.Out);
				return;
			}

			options.Parse(args);

			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			var repository = new EngineRepository(folder);

			if (mode == "train")
				Train(repository);
			else if (mode == "play")
				Play(repository, engine);
			else if (mode == "human")
				PlayAgainsHuman(repository, engine);
			else
				Console.WriteLine("The mode should either be 'train', 'play' or 'human'");
		}

		static void Train(EngineRepository repository)
		{
			int iteration = 0;

			while (true)
			{
				Console.WriteLine($"Running iteration { iteration }");

				repository.Purge(80);
				var tournament = new Tournament(repository, 80);
				tournament.Train(10);

				iteration++;
			}
		}

		static void Play(EngineRepository repository, string engineId)
		{
			IEngine trained;

			if (String.IsNullOrEmpty(engineId))
				trained = repository.GetTopEngines(1).First();
			else
				trained = repository.Load(engineId);

			int White = 0;
			int Black = 0;

			do
			{
				IEngine randomEngine = repository.CreateRandom();
				var game = new Game(randomEngine, trained);
				var printer = new TicTacToePrinter(game.Board);

				CellValue color = game.Play();

				if (color == CellValue.White)
					White++;
				else if (color == CellValue.Black)
					Black++;

				Console.Write(printer.Print());

				Console.WriteLine($"{color} has won");
				Console.WriteLine($"White: {White} Black: {Black}");
			}
			while (Console.ReadKey(true).KeyChar != 'q');
		}

		static void PlayAgainsHuman(EngineRepository repository, string engineId)
		{
			IEngine engine;
			IEngine human = new Human();

			if (String.IsNullOrEmpty(engineId))
				engine = repository.GetTopEngines(1).First();
			else
				engine = repository.Load(engineId);

			var game = new Game(human, engine);
			var color = game.Play();
			var printer = new TicTacToePrinter(game.Board);

			Console.Write(printer.Print());
			Console.WriteLine($"{color} has won");
			Console.ReadKey(true);
		}
	}
}