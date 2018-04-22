using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;
using Random = System.Random;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Zenject;

namespace App.Scripts.Boards
{
    public class GameBoard : IGameBoard
    {
        [Inject] private CellData.Factory cellDataFactory;
        [Inject] private IList<CellData> Cells { get; set; }
        [Inject] private IReactiveProperty<GameStatus> Status { get; set; }
        [Inject] public GameSetting GameSettings { get; set; }
        
        public int Width
        {
            get { return GameSettings.Width; }
            set { GameSettings.Width = value; }
        }
        public int Height
        {
            get { return GameSettings.Height; }
            set { GameSettings.Height = value; }
        }
        public int MineCount
        {
            get { return GameSettings.MineCount; }
            set { GameSettings.MineCount = value; }
        }

        public void Build(int width, int height, int mines, IList<CellData> cells)
        {
            GameSettings.Width = width;
            GameSettings.Height = height;
            GameSettings.MineCount = mines;

            cells.Clear();

            var id = 1;
            for (var i = 1; i <= height; i++)
            {
                for (var j = 1; j <= width; j++)
                {
                    var cell = cellDataFactory.Create(id, j, i);
                    cells.Add(cell);
                    id++;
                }
            }
        }

        public CellData GetCellAt(int x, int y)
        {
            return Cells.First(z => z.X == x && z.Y == y);
        }

        public void Open(int x, int y)
        {
            //Find the Specified CellData
            var selectedCell = Cells.First(cell => cell.X == x && cell.Y == y);
            selectedCell.IsRevealed.Value = true;
            selectedCell.IsFlagged.Value = false; //Revealed cells cannot be flagged

            //If the panel is a mine, game over!
            if (selectedCell.IsMine.Value)
            {
                Status.Value = GameStatus.Failed;
            }

            //If the panel is a zero, cascade reveal neighbors
            if (!selectedCell.IsMine.Value && selectedCell.AdjacentMines.Value == 0)
            {
                OpenEmptyCell(x, y);
            }

            //If this move caused the game to be complete, mark it as such
            if (!selectedCell.IsMine.Value)
            {
                CheckForCompletion();
            }
        }

        private void OpenEmptyCell(int x, int y)
        {
            var neighborCells = GetNeighbors(x, y).Where(panel => !panel.IsRevealed.Value);
            foreach (var neighbor in neighborCells)
            {
                neighbor.IsRevealed.Value = true;
                if (neighbor.AdjacentMines.Value == 0)
                {
                    OpenEmptyCell(neighbor.X, neighbor.Y);
                }
            }
        }


        public void FirstMove(int x, int y, Random rand)
        {
            var neighbors = GetNeighbors(x, y);
            neighbors.Add(GetCellAt(x, y)); //Don't place a mine in the user's first move!

            //Select random cells from set of cells which are not excluded by the first-move rule
            var mineList = Cells.Except(neighbors).OrderBy(user => rand.Next());
            var mineSlots = mineList.Take(MineCount).ToList().Select(z => new {z.X, z.Y});

            //Place the mines
            foreach (var mineCoord in mineSlots)
            {
                Cells.Single(cell => cell.X == mineCoord.X && cell.Y == mineCoord.Y).IsMine.Value = true;
            }

            //For every panel which is not a mine, determine and save the adjacent mines.
            foreach (var openCell in Cells.Where(cell => !cell.IsMine.Value))
            {
                var nearbyCells = GetNeighbors(openCell.X, openCell.Y);
                openCell.AdjacentMines.Value = nearbyCells.Count(z => z.IsMine.Value);
            }
        }

        public List<CellData> GetNeighbors(int x, int y, int depth = 1)
        {
            var nearbyCells = Cells.Where(cell => cell.X >= (x - depth) && cell.X <= (x + depth)
                                                                        && cell.Y >= (y - depth) &&
                                                                        cell.Y <= (y + depth));
            var currentCell = Cells.Where(cell => cell.X == x && cell.Y == y);
            return nearbyCells.Except(currentCell).ToList();
        }


        private void CheckForCompletion()
        {
            var hiddenCells = Cells.Where(x => !x.IsRevealed.Value).Select(x => x.ID);
            var mineCells = Cells.Where(x => x.IsMine.Value).Select(x => x.ID);
            if (!hiddenCells.Except(mineCells).Any())
            {
                Status.Value = GameStatus.Completed;
            }
        }

        public void Flag(int x, int y)
        {
            var cell = Cells.FirstOrDefault(z => z.X == x && z.Y == y);
            if (cell != null && !cell.IsRevealed.Value)
            {
                cell.IsFlagged.Value = true;
            }
        }

    }
}