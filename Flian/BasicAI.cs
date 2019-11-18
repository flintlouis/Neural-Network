using System;
namespace Flian
{
	/*
    public class BasicAI : IEngine
	{
		private readonly NNEngine network;

		public BasicAI()
		{
			var config = new EngineConfig
			{
				InputSize = 64, // Input layer size
				OutputSize = 64, // Output layer size
				LayerCount = 5, // Total layers, input layer + 3 hidden layers 
				LayerSize = 48, // Hidden layers size
			};

			this.network = new NNEngine(new NeuralNetwork(config));
		}

		public Position Calculate(Board board, CellValue color) // Ai calculate move
		{


			if (ai.CalculateMove(this.board))
				return;


			var engine = new Engine(config);
			engine.Randomize(new Random());
			this.CalculateMove(engine, ai.Value);
		}


		private bool CalculateMove(CellValue[,,] board) // First checks if can win, if not checks if he can stop opponent from winning if not place random stone
        {
            CellValue value = this.Value;
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (CheckLayer(board, value, i))
                        return true;
                }
                if (Check3D(board, value))
                    return true;
                value = value == CellValue.White ? CellValue.Black : CellValue.White;
            }
            return false;
        }

        private bool Check3D(CellValue[,,] board, CellValue value)
        {
            int tiar;

            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    tiar = 0;

                    if (board[0, j, i] == value)
                        tiar++;
                    if (board[1, j, i] == value)
                        tiar++;
                    if (board[2, j, i] == value)
                        tiar++;
                    if (board[3, j, i] == value)
                        tiar++;
                    if (tiar == 3)
                    {
                        return StopWin3D(board, j, i);
                    }
                }
                tiar = 0;

                if (board[0, 0, j] == value)
                    tiar++;
                if (board[1, 1, j] == value)
                    tiar++;
                if (board[2, 2, j] == value)
                    tiar++;
                if (board[3, 3, j] == value)
                    tiar++;
                if (tiar == 3)
                {
                    return StopWin3DSideZ(board, j, 0, 1, 2, 3);
                }
                tiar = 0;

                if (board[0, 3, j] == value)
                    tiar++;
                if (board[1, 2, j] == value)
                    tiar++;
                if (board[2, 1, j] == value)
                    tiar++;
                if (board[3, 0, j] == value)
                    tiar++;
                if (tiar == 3)
                {
                    return StopWin3DSideZ(board, j, 3, 2, 1, 0);
                }
                tiar = 0;

                if (board[0, j, 0] == value)
                    tiar++;
                if (board[1, j, 1] == value)
                    tiar++;
                if (board[2, j, 2] == value)
                    tiar++;
                if (board[3, j, 3] == value)
                    tiar++;
                if (tiar == 3)
                {
                    return StopWin3DSideY(board, j, 0, 1, 2, 3);
                }
                tiar = 0;

                if (board[0, j, 3] == value)
                    tiar++;
                if (board[1, j, 2] == value)
                    tiar++;
                if (board[2, j, 1] == value)
                    tiar++;
                if (board[3, j, 0] == value)
                    tiar++;
                if (tiar == 3)
                {
                    return StopWin3DSideY(board, j, 3, 2, 1, 0);
                }
            }
            tiar = 0;

            for (int i = 0; i < 4; i++)
            {
                if (board[i, i, i] == value)
                    tiar++;
            }
            if (tiar == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (IsEmpty(board, i, i, i))
                    {
                        board[i, i, i] = this.Value;
                        return true;
                    }
                }
            }

            tiar = 0;
            int k = 3;
            for (int i = 0; i < 4; i++)
            {
                if (board[i, k, k] == value)
                    tiar++;
                k--;
            }
            if (tiar == 3)
            {
                k = 3;
                for (int i = 0; i < 4; i++)
                {
                    if (IsEmpty(board, i, k, k))
                    {
                        board[i, k, k] = this.Value;
                        return true;
                    }
                    k--;
                }
            }
            tiar = 0;

            k = 3;
            for (int i = 0; i < 4; i++)
            {
                if (board[i, k, i] == value)
                    tiar++;
                k--;
            }
            if (tiar == 3)
            {
                k = 3;
                for (int i = 0; i < 4; i++)
                {
                    if (IsEmpty(board, i, k, i))
                    {
                        board[i, k, i] = this.Value;
                        return true;
                    }
                    k--;
                }
            }
            tiar = 0;

            k = 3;
            for (int i = 0; i < 4; i++)
            {
                if (board[i, i, k] == value)
                    tiar++;
                k--;
            }
            if (tiar == 3)
            {
                k = 3;
                for (int i = 0; i < 4; i++)
                {
                    if (IsEmpty(board, i, i, k))
                    {
                        board[i, i, k] = this.Value;
                        return true;
                    }
                    k--;
                }
            }
            return false;
        }

        private bool StopWin3DSideZ(CellValue[,,] board, int z, int y1, int y2, int y3, int y4)
        {

            if (IsEmpty(board, 0, y1, z))
            {
                board[0, y1, z] = this.Value;
                return true;
            }
            if (IsEmpty(board, 1, y2, z))
            {
                board[1, y2, z] = this.Value;
                return true;
            }
            if (IsEmpty(board, 2, y3, z))
            {
                board[2, y3, z] = this.Value;
                return true;
            }
            if (IsEmpty(board, 3, y4, z))
            {
                board[3, y4, z] = this.Value;
                return true;
            }
            return false;
        }

        private bool StopWin3DSideY(CellValue[,,] board, int y, int z1, int z2, int z3, int z4)
        {

            if (IsEmpty(board, 0, y, z1))
            {
                board[0, y, z1] = this.Value;
                return true;
            }
            if (IsEmpty(board, 1, y, z2))
            {
                board[1, y, z2] = this.Value;
                return true;
            }
            if (IsEmpty(board, 2, y, z3))
            {
                board[2, y, z3] = this.Value;
                return true;
            }
            if (IsEmpty(board, 3, y, z4))
            {
                board[3, y, z4] = this.Value;
                return true;
            }
            return false;
        }

        private bool StopWin3D(CellValue[,,] board, int y, int z)
        {

            for (int i = 0; i < 4; i++)
            {
                if (IsEmpty(board, i, y, z))
                {
                    board[i, y, z] = this.Value;
                    return true;
                }
            }
            return false;
        }

        private bool CheckLayer(CellValue [,,] board, CellValue value, int layer)
        {
            int tiar;
            for (int i = 0; i < 4; i++)
            {
                tiar = 0;
                if (board[layer, 0, i] == value)
                    tiar++;
                if (board[layer, 1, i] == value)
                    tiar++;
                if (board[layer, 2, i] == value)
                    tiar++;
                if (board[layer, 3, i] == value)
                    tiar++;
                if (tiar == 3)
                {
                    return StopWinY(board, layer, i);
                }
                tiar = 0;
                if (board[layer, i, 0] == value)
                    tiar++;
                if (board[layer, i, 1] == value)
                    tiar++;
                if (board[layer, i, 2] == value)
                    tiar++;
                if (board[layer, i, 3] == value)
                    tiar++;
                if (tiar == 3)
                {
                    return StopWinZ(board, layer, i);
                }
            }

            tiar = 0;
            if (board[layer, 0, 0] == value)
                tiar++;
            if (board[layer, 1, 1] == value)
                tiar++;
            if (board[layer, 2, 2] == value)
                tiar++;
            if (board[layer, 3, 3] == value)
                tiar++;
            if (tiar == 3)
            {
                return StopWin(board, layer, 0, 1, 2, 3);
            }

            tiar = 0;
            if (board[layer, 0, 3] == value)
                tiar++;
            if (board[layer, 1, 2] == value)
                tiar++;
            if (board[layer, 2, 1] == value)
                tiar++;
            if (board[layer, 3, 0] == value)
                tiar++;
            if (tiar == 3)
            {
                return StopWin(board, layer, 3, 2, 1, 0);
            }
            return false;
        }

        private bool StopWin(CellValue[,,] board, int x, int z1, int z2, int z3, int z4)
        {

            if (IsEmpty(board, x, 0, z1))
            {
                board[x, 0, z1] = this.Value;
                return true;
            }
            else if (IsEmpty(board, x, 1, z2))
            {
                board[x, 1, z2] = this.Value;
                return true;
            }
            else if (IsEmpty(board, x, 2, z3))
            {
                board[x, 2, z3] = this.Value;
                return true;
            }
            else if (IsEmpty(board, x, 3, z4))
            {
                board[x, 3, z4] = this.Value;
                return true;
            }
            else
                return false;
        }

        private bool StopWinY(CellValue[,,] board, int x, int z)
        {

            for (int i = 0; i < 4; i++)
            {
                if (IsEmpty(board, x, i, z))
                {
                    board[x,i,z] = this.Value;
                    return true;
                }
            }
            return false;
        }

        private bool StopWinZ(CellValue[,,] board, int x, int y)
        {

            for (int i = 0; i < 4; i++)
            {
                if (IsEmpty(board, x, y, i))
                {
                    board[x, y, i] = this.Value;
                    return true;
                }
            }
            return false;
        }

        private bool IsEmpty(CellValue[,,] board, int x, int y, int z)
        {
            return board[x, y, z] == CellValue.Empty;
        }
    }
	*/
}
