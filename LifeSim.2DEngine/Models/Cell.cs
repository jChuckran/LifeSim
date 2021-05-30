using System.Collections.Generic;
using System.Linq;
using LifeSim.Engine2D.Helpers;
namespace LifeSim.Engine2D.Models
{
    public class Cell : INotifyPropertyChangedEx
    {
        private bool _isAlive;
        public bool IsAlive
        {
            get { return _isAlive; }
            set
            {
                _isAlive = value;
                OnPropertyChanged();
            }
        }

        public bool IsAliveNext { get; set; }

        public long X { get; set; }
        public long Y { get; set; }

        private CellCollection _cellCollection;

        public List<Cell> Neighbors { get; set; } = new List<Cell>();

        public bool AnyLivingNeighbors
        {
            get
            {
                if (Neighbors == null)
                    return false;
                else
                    return Neighbors.Any(c => c.IsAlive);
            }
        }

        public Cell(CellCollection cellCollection, long x, long y, bool isAlive)
        {
            _cellCollection = cellCollection;
            X = x;
            Y = y;
            IsAlive = isAlive;
        }

        public void CheckForNewNeighbors()
        {
            _cellCollection.GetNewNeighbors(Neighbors, X, Y);
        }

        public void DetermineNextLiveState()
        {
            //Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            //Any live cell with more than three live neighbours dies, as if by overpopulation.
            //Any live cell with two or three live neighbours lives on to the next generation.
            //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.         

            int liveNeighbors = Neighbors.Where(x => x.IsAlive).Count();

            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }

        public void Advance()
        {
            IsAlive = IsAliveNext;
            if (IsAlive)
                CheckForNewNeighbors();
        }
    }
}