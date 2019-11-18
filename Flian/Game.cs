using System;

namespace Flian
{
	public class Game
	{
		private readonly IEngine player1;
		private readonly IEngine player2;

		public Game(IEngine player1, IEngine player2)
		{
			this.Board = new Board();
			this.player1 = player1;
			this.player2 = player2;
		}

		public CellValue Play()
		{
			CellValue color = CellValue.White;

			while (!this.Board.IsFull)
			{
				Position position;

				if (color == CellValue.White)
					position = this.player1.Calculate(this.Board, color);
				else
					position = this.player2.Calculate(this.Board, color);

				this.Board[position] = color;

				if (this.Board.HasWon(color))
					return color;

				color = color == CellValue.Black ? CellValue.White : CellValue.Black;
			}

			return CellValue.Empty;
		}

		public Board Board { get; }
	}
}