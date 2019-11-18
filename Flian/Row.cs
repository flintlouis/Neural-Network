using System;

namespace Flian
{
	public struct Row
	{
		private readonly Board board;
		private readonly Position[] positions;

		public Row(Board board, Position[] positions)
		{
			if (positions.Length != 4)
				throw new ArgumentException("Expected 4 positions", nameof(positions));

			this.board = board;
			this.positions = positions;
		}

		public Position this[int index] => this.positions[index];

		public CellValue GetValue(int index) => this.board[this[index]];

		public int Count(CellValue value)
		{
			int counter = 0;

			for (int i = 0; i < 4; i++)
			{
				if (this.GetValue(i) == value)
					counter++;
			}

			return counter;
		}

		public bool HasFullRow(CellValue value) => this.Count(value) == 4;
	}
}