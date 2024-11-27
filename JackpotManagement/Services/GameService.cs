using System;

namespace GameManagement.Services
{
    public class GameService : IGameService
    {
        public bool CheckWin(char[,] grid)
        {
            if (grid.GetLength(0) != 3 || grid.GetLength(1) != 3)
                throw new ArgumentException("Grid must be a 3x3 matrix.");

            for (int i = 0; i < 3; i++)
            {
                if (grid[i, 0] == grid[i, 1] && grid[i, 1] == grid[i, 2] && grid[i, 0] != ' ')
                    return true; // Row match
                if (grid[0, i] == grid[1, i] && grid[1, i] == grid[2, i] && grid[0, i] != ' ')
                    return true; // Column match
            }

            if (grid[0, 0] == grid[1, 1] && grid[1, 1] == grid[2, 2] && grid[0, 0] != ' ')
                return true; // Top-left to bottom-right diagonal
            if (grid[0, 2] == grid[1, 1] && grid[1, 1] == grid[2, 0] && grid[0, 2] != ' ')
                return true; // Top-right to bottom-left diagonal

            return false; // No winning combination
        }
    }
}
