using System;
using System.Collections;
using System.Collections.Generic;

namespace Flian
{
	public class Board
	{
		private readonly CellValue[,,] board; // Playing board
		private int freeCells = 64; // Amount of free places on game board to start with

		public Board()
		{
			this.board = new CellValue[4,4,4];
		}

		public CellValue this[Position position]
		{
			get => this.board[position.X, position.Y, position.Z];

			set
			{
				if (this[position] == CellValue.Empty)
					this.freeCells--;
				else if (value == CellValue.Empty)
					this.freeCells++;

				this.board[position.X, position.Y, position.Z] = value;
			}
		}

		public CellValue this[int x, int y, int z]
		{
			get => this.board[x, y, z];
			set => this[new Position(x, y, z)] = value;
		}

		public bool IsFull => this.freeCells == 0;

		public IEnumerable<Row> GetAllRows()
		{
			for (int layer = 0; layer < 4; layer++)
			{
				foreach (Row row in this.GetAllRowsOfLayer(layer))
					yield return row;
			}

			foreach (Row row in this.GetAll3DRows())
				yield return row;
		}

		private IEnumerable<Row> GetAllRowsOfLayer(int layer)
		{
			for (int i = 0; i < 4; i++)
			{
				yield return new Row(this, new[] {
					new Position(layer, 0, i),
					new Position(layer, 1, i),
					new Position(layer, 2, i),
					new Position(layer, 3, i),
				});

				yield return new Row(this, new[] {
					new Position(layer, i, 0),
					new Position(layer, i, 1),
					new Position(layer, i, 2),
					new Position(layer, i, 3),
				});
			}

			yield return new Row(this, new[] {
				new Position(layer, 0, 0),
				new Position(layer, 1, 1),
				new Position(layer, 2, 2),
				new Position(layer, 3, 3),
			});

			yield return new Row(this, new[] {
				new Position(layer, 3, 0),
				new Position(layer, 2, 1),
				new Position(layer, 1, 2),
				new Position(layer, 0, 3),
			});
		}

		private IEnumerable<Row> GetAll3DRows()
		{
			for (int j = 0; j < 4; j++)
			{
				for (int i = 0; i < 4; i++)
				{
					// Straight down
					yield return new Row(this, new[] {
						new Position(0, i, j),
						new Position(1, i, j),
						new Position(2, i, j),
						new Position(3, i, j),
					});
				}

				yield return new Row(this, new[] {
					new Position(0, 0, j),
					new Position(1, 1, j),
					new Position(2, 2, j),
					new Position(3, 3, j),
				});

				yield return new Row(this, new[] {
					new Position(0, 3, j),
					new Position(1, 2, j),
					new Position(2, 1, j),
					new Position(3, 0, j),
				});

				yield return new Row(this, new[] {
					new Position(0, j, 0),
					new Position(1, j, 1),
					new Position(2, j, 2),
					new Position(3, j, 3),
				});

				yield return new Row(this, new[] {
					new Position(0, j, 3),
					new Position(1, j, 2),
					new Position(2, j, 1),
					new Position(3, j, 0),
				});
			}

			yield return new Row(this, new[] {
				new Position(0, 0, 0),
				new Position(1, 1, 1),
				new Position(2, 2, 2),
				new Position(3, 3, 3),
			});

			yield return new Row(this, new[] {
				new Position(0, 3, 3),
				new Position(1, 2, 2),
				new Position(2, 1, 1),
				new Position(3, 0, 0),
			});

			yield return new Row(this, new[] {
				new Position(0, 0, 3),
				new Position(1, 1, 2),
				new Position(2, 2, 1),
				new Position(3, 3, 0),
			});

			yield return new Row(this, new[] {
				new Position(0, 3, 0),
				new Position(1, 2, 1),
				new Position(2, 1, 2),
				new Position(3, 0, 3),
			});
		}

		public bool HasWon(CellValue value) // Check if black or white has 4 in a row
		{
			if (value == CellValue.Empty)
				throw new ArgumentException(nameof(value));

			foreach (Row row in this.GetAllRows())
			{
				if (row.HasFullRow(value))
					return true;
			}

			/*

			if (this.Check3D(value))
				return true;

			for (int i = 0; i < 4; i++)
				if (this.HasWon(value, i))
					return true;

			*/
			return false;
		}

		/*

		private bool Check3D(CellValue value)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int i = 0; i < 4; i++)
				{
					if (AreEqual(value, this.board[0, j, i], this.board[1, j, i], this.board[2, j, i], this.board[3, j, i]))
						return true;
				}
				if (AreEqual(value, this.board[0, 0, j], this.board[1, 1, j], this.board[2, 2, j], this.board[3, 3, j]))
					return true;
				if (AreEqual(value, this.board[0, 3, j], this.board[1, 2, j], this.board[2, 1, j], this.board[3, 0, j]))
					return true;
				if (AreEqual(value, this.board[0, j, 0], this.board[1, j, 1], this.board[2, j, 2], this.board[3, j, 3]))
					return true;
				if (AreEqual(value, this.board[0, j, 3], this.board[1, j, 2], this.board[2, j, 1], this.board[3, j, 0]))
					return true;
			}

			return AreEqual(value, this.board[0, 0, 0], this.board[1, 1, 1], this.board[2, 2, 2], this.board[3, 3, 3])
				|| AreEqual(value, this.board[0, 3, 3], this.board[1, 2, 2], this.board[2, 1, 1], this.board[3, 0, 0])
				|| AreEqual(value, this.board[0, 3, 0], this.board[1, 2, 1], this.board[2, 1, 2], this.board[3, 0, 3])
				|| AreEqual(value, this.board[0, 0, 3], this.board[1, 1, 2], this.board[2, 2, 1], this.board[3, 3, 0]);
		}

		private bool HasWon(CellValue value, int layer)
		{
			for (int i = 0; i < 4; i++)
			{
				if (AreEqual(value, this.board[layer, 0, i], this.board[layer, 1, i], this.board[layer, 2, i], this.board[layer, 3, i]))
					return true;

				if (AreEqual(value, this.board[layer, i, 0], this.board[layer, i, 1], this.board[layer, i, 2], this.board[layer, i, 3]))
					return true;
			}

			return AreEqual(value, this.board[layer, 0, 0], this.board[layer, 1, 1], this.board[layer, 2, 2], this.board[layer, 3, 3])
				|| AreEqual(value, this.board[layer, 0, 3], this.board[layer, 1, 2], this.board[layer, 2, 1], this.board[layer, 3, 0]);
		}

		private static bool AreEqual(CellValue value1, CellValue value2, CellValue value3, CellValue value4,
			CellValue value5)
		{
			return value1 == value2 && value1 == value3 && value1 == value4 && value1 == value5;
		}
		*/
	}
}
