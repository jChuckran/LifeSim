using LifeSim.Engine2D.Models;
using System.Linq;

namespace LifeSim.Engine2D.Rules
{
    public class UncheckedGrowthGameOfLife : IRules
    {
        public bool GetNextLivingState(TrackedCell cell)
        {
            //Any live cell with fewer than two live neighbors dies, as if by underpopulation.
            //Any live cell with more than six live neighbors dies, as if by overpopulation.
            //Any live cell with two to six live neighbors lives on to the next generation.
            //Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.         

            int liveNeighbors = cell.Neighbors.Where(x => x.IsAlive).Count();

            if (cell.IsAlive)
            {
                return liveNeighbors >= 2 && liveNeighbors <= 6;
            }
            else
            {
                return liveNeighbors == 3;
            }

        }
    }
}
