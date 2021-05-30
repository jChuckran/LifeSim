using LifeSim.Engine2D.Rules;
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

        public void DetermineNextState(IRules rules)
        {
            IsAliveNext = rules.GetNextLivingState(this);
        }

        public void Advance()
        {
            IsAlive = IsAliveNext;
            if (IsAlive)
                CheckForNewNeighbors();
        }
    }
}