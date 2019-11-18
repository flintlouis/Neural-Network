using System;

namespace Flian
{
	public class Human : IEngine
	{
		public Position Calculate(Board board, CellValue color)
		{
			if (color == CellValue.Empty)
				throw new ArgumentException(nameof(color));

			int x, y, z;

			while(true)
			{
				TicTacToePrinter printer = new TicTacToePrinter(board);
				Console.WriteLine(printer.Print());

				Console.Write("position: ");
				string pos = Console.ReadLine();

				x = Int32.Parse(pos[1].ToString());
				y = Int32.Parse(pos[0].ToString());
				z = Int32.Parse(pos[2].ToString());

				var position = new Position(x, y, z);

				if (board[position] == CellValue.Empty)
					return position;

				Console.WriteLine("Try again");
			}
		}
	}
}