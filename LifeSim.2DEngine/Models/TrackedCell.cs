using LifeSim.Engine2D.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace LifeSim.Engine2D.Models
{
    public class TrackedCell : Cell
    {
        [JsonIgnore]
        public bool IsAliveNext { get; set; }

        private CellCollection _cellCollection;

        [JsonIgnore]
        public List<TrackedCell> Neighbors { get; set; } = new List<TrackedCell>();

        [JsonIgnore]
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

        public TrackedCell(CellCollection cellCollection, long x, long y, bool isAlive)
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