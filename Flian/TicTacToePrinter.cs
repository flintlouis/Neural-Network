using System.Text;

namespace Flian
{
	public class TicTacToePrinter // Prints board/game
	{
		private readonly Board board;

		public TicTacToePrinter(Board board)
		{
			this.board = board;
		}

		public string Print()
		{
			var output = new StringBuilder();

			for (int x=0;x<4;x++) {
				output.AppendLine("+---+---+---+---+  +---+---+---+---+  +---+---+---+---+  +---+---+---+---+");

				for (int z = 0; z < 4; z++)
				{
					for (int y = 0; y < 4; y++)
					{
						output.Append("|");
						CellValue value = this.board[x, y, z];
						switch (value)
						{
							case CellValue.Black:
								output.Append(" X ");
								break;
							case CellValue.White:
								output.Append(" O ");
								break;
							default:
								output.Append("   ");
								break;
						}
					}

					output.Append("|  ");
				}
				output.AppendLine();
			}

			output.AppendLine("+---+---+---+---+  +---+---+---+---+  +---+---+---+---+  +---+---+---+---+");

			return output.ToString();
		}
	}
}
