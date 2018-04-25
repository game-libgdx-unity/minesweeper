using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace App.Scripts.Boards
{

    public class GameSolver : IGameSolver
    {
        [Inject] private IReactiveProperty<GameStatus> Status { get; set; }

        [Inject] private IList<CellData> CellData { get; set; }

        [Inject] private IGameBoard Board { get; set; }

        [Inject] private Random Random { get; set; }

        public IEnumerator Solve(float waitForNextStep)
        {
            yield return new WaitForSeconds(waitForNextStep);

            Debug.Log("New Turn");

            while (Status.Value == GameStatus.InProgress)
            {
                if (!CellData.Any(x => x.IsRevealed.Value))
                {
                    RandomFirstMove();
                    yield return new WaitForSeconds(waitForNextStep);
                }

                FlagObviousMines();
                yield return new WaitForSeconds(waitForNextStep);

                if (HasAvailableMoves())
                {
                    CalculateNumberOfMines();
                    yield return new WaitForSeconds(waitForNextStep);
                }
                else
                {
                    RandomMove();
                    yield return new WaitForSeconds(waitForNextStep);
                }

                EndTurn_OpenAllCells();
            }

            RevealAllMines();

            if (Status.Value == GameStatus.Failed)
            {
                Debug.Log("Solver Failed!");
            }
            else if (Status.Value == GameStatus.Completed)
            {
                Debug.Log("Solver Success");
            }
        }

        private void RevealAllMines()
        {
            Debug.Log("RevealAllMines");

            foreach (var cell in CellData)
            {
                cell.IsRevealed.Value = true;
            }
        }

        private void RandomFirstMove()
        {
            Debug.Log("RandomFirstMove");

            var randomX = Random.Next(1, Board.Width - 1);
            var randomY = Random.Next(1, Board.Height - 1);

            Board.FirstMove(randomX, randomY, Random);
            Board.Open(randomX, randomY);
        }

        private void RandomMove()
        {
            Debug.Log("RandomMove");

            var randomID = Random.Next(1, CellData.Count);
            var cell = CellData.First(x => x.ID == randomID);
            while (cell.IsRevealed.Value || cell.IsFlagged.Value)
            {
                randomID = Random.Next(1, CellData.Count);
                cell = CellData.First(x => x.ID == randomID);
            }

            Board.Open(cell.X, cell.Y);
        }

        private bool HasAvailableMoves()
        {
            var numberedCells = CellData.Where(x => x.IsRevealed.Value && x.AdjacentMines.Value > 0);
            foreach (var numberPanel in numberedCells)
            {
                var neighborCells = Board.GetNeighbors(numberPanel.X, numberPanel.Y);
                var flaggedNeighbors = neighborCells.Where(x => x.IsFlagged.Value);
                if (flaggedNeighbors.Count() == numberPanel.AdjacentMines.Value &&
                    neighborCells.Any(x => !x.IsRevealed.Value && !x.IsFlagged.Value))
                {
                    return true;
                }
            }

            return false;
        }

        private void CalculateNumberOfMines()
        {
            Debug.Log("CalculateNumberOfMines");

            var numberedCells = CellData.Where(x => x.IsRevealed.Value && x.AdjacentMines.Value > 0);
            foreach (var numberCell in numberedCells)
            {
                var neighborCells = Board.GetNeighbors(numberCell.X, numberCell.Y);

                var flaggedNeighbors = neighborCells.Where(x => x.IsFlagged.Value);

                if (flaggedNeighbors.Count() == numberCell.AdjacentMines.Value)
                {
                    foreach (var hiddenPanel in neighborCells.Where(x => !x.IsRevealed.Value && !x.IsFlagged.Value))
                    {
                        Board.Open(hiddenPanel.X, hiddenPanel.Y);
                    }
                }
            }
        }

        private void FlagObviousMines()
        {
            Debug.Log("FlagObviousMines");

            var numberCells = CellData.Where(x => x.IsRevealed.Value && x.AdjacentMines.Value > 0);
            foreach (var cell in numberCells)
            {
                var neighborCells = Board.GetNeighbors(cell.X, cell.Y);

                if (neighborCells.Count(x => !x.IsRevealed.Value) == cell.AdjacentMines.Value)
                {
                    foreach (var neighbor in neighborCells.Where(x => !x.IsRevealed.Value))
                    {
                        Board.Flag(neighbor.X, neighbor.Y);
                    }
                }
            }
        }

        private void EndTurn_OpenAllCells()
        {
            Debug.Log("EndTurn_OpenAllCells");

            var flaggedCells = CellData.Count(x => x.IsFlagged.Value);
            if (flaggedCells == Board.MineCount)
            {
                var hiddenCells = CellData.Where(x => !x.IsFlagged.Value && !x.IsRevealed.Value);
                foreach (var cell in hiddenCells)
                {
                    Board.Open(cell.X, cell.Y);
                }
            }
        }
    }
}