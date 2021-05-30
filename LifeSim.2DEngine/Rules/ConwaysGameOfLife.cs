using LifeSim.Engine2D.Models;
using System.Linq;

namespace LifeSim.Engine2D.Rules
{
    public class ConwaysGameOfLife : IRules
    {
        public bool GetNextLivingState(TrackedCell cell)
        {
            //Any live cell with fewer than two live neighbours dies, as if by underpopulation.
            //Any live cell with more than three live neighbours dies, as if by overpopulation.
            //Any live cell with two or three live neighbours lives on to the next generation.
            //Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.         

            int liveNeighbors = cell.Neighbors.Where(x => x.IsAlive).Count();

            if (cell.IsAlive)
                return liveNeighbors == 2 || liveNeighbors == 3;
            else
                return liveNeighbors == 3;

        }
    }
}
