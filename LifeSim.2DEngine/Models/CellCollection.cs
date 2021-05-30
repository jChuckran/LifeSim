using LifeSim.Engine2D.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LifeSim.Engine2D.Models
{
    public class CellCollection : List<TrackedCell>
    {
        private object cellSync = new object();

        public string Export()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public void Import(string cellJson)
        {
            ClearCells();
            var cells = JsonConvert.DeserializeObject<IEnumerable<Cell>>(cellJson);
            foreach (Cell cell in cells)
            {
                UpdateCell(cell.X, cell.Y, cell.IsAlive);
            }
        }

        public void ClearCells()
        {
            lock (cellSync)
            {
                Clear();
            }
        }

        public async Task Advance()
        {
            await Task.Run(() =>
            {
                lock (cellSync)
                {
                    var allCells = this.ToList();
                    foreach (TrackedCell cell in allCells)
                    {
                        cell.DetermineNextLiveState();
                    }
                    foreach (TrackedCell cell in allCells)
                    {
                        cell.Advance();
                    }
                    List<TrackedCell> untrackedCells = new List<TrackedCell>();
                    foreach (TrackedCell cell in allCells)
                    {
                        if (!cell.IsAlive && !cell.AnyLivingNeighbors)
                        {
                            untrackedCells.Add(cell);
                        }
                    }
                    if (untrackedCells.Any())
                    {
                        foreach (TrackedCell cell in untrackedCells)
                        {
                            foreach (TrackedCell neighbor in cell.Neighbors)
                            {
                                if (neighbor.Neighbors.Contains(cell))
                                    neighbor.Neighbors.Remove(cell);
                            }
                            Remove(cell);
                        }
                    }
                }
            });
        }

        public void Randomize(double liveDensity, long startX, long endX, long startY, long endY)
        {
            Random rand = new Random();
            for (long x = startX; x <= endX; x++)
            {
                for (long y = startY; y <= endY; y++)
                {
                    if (rand.NextDouble() < liveDensity)
                    {
                        AddLivingCell(x, y);
                    }
                }
            }

        }

        public void AddExisting(List<TrackedCell> existing, long x, long y)
        {
            var res = this.Where((c) => c.X == x && c.Y == y);
            if (res.Any())
                existing.Add(res.First());
        }

        public void AddLivingCell(long x, long y)
        {
            lock (cellSync)
            {
                GetOrAddCell(x, y, true, true);
            }
        }

        public void UpdateCell(long x, long y, bool isAlive)
        {
            lock (cellSync)
            {
                GetOrAddCell(x, y, isAlive, true);
            }
        }

        public void ToggleCell(long x, long y)
        {
            lock (cellSync)
            {
                var res = this.Where((c) => c.X == x && c.Y == y);
                if (res.Any())
                {
                    var c = res.First();
                    c.IsAlive = !c.IsAlive;
                }
                else
                    GetOrAddCell(x, y, true, true);
            }
        }

        public TrackedCell GetOrAddCell(long x, long y, bool newIsAlive, bool updateExisting = false)
        {
            List<TrackedCell> existing = new List<TrackedCell>();

            //Row 1
            AddExisting(existing, x - 1, y - 1);
            AddExisting(existing, x, y - 1);
            AddExisting(existing, x + 1, y - 1);
            //Row 2
            AddExisting(existing, x - 1, y);
            AddExisting(existing, x, y);
            AddExisting(existing, x + 1, y);
            //Row 3
            AddExisting(existing, x - 1, y + 1);
            AddExisting(existing, x, y + 1);
            AddExisting(existing, x + 1, y + 1);

            //var existing = this.Where((c) => (c.X >= x - 1 || c.X <= x + 1) && (c.Y >= y - 1 || c.Y <= y + 1)).ToList();

            if (existing.Any((c) => c.X == x && c.Y == y))
            {
                var c = existing.Where((c) => c.X == x && c.Y == y).First();
                if (updateExisting)
                {
                    c.IsAlive = newIsAlive;
                    if (newIsAlive)
                        c.CheckForNewNeighbors();
                }
                return c;
            }
            var newCell = new TrackedCell(this, x, y, newIsAlive);
            if (existing.Any())
            {
                foreach (var n in existing)
                {
                    n.Neighbors.Add(newCell);
                }
                newCell.Neighbors.AddRange(existing);
            }
            Add(newCell);
            if (newIsAlive)
                newCell.CheckForNewNeighbors();
            return newCell;
        }

        private void AddNeighbor(List<TrackedCell> currentNeighbors, long x, long y)
        {
            if (!currentNeighbors.Any((c) => c.X == x && c.Y == y))
                GetOrAddCell(x, y, false);
        }

        public void GetNewNeighbors(List<TrackedCell> currentNeighbors, long x, long y)
        {
            //Row 1
            AddNeighbor(currentNeighbors, x - 1, y - 1);
            AddNeighbor(currentNeighbors, x, y - 1);
            AddNeighbor(currentNeighbors, x + 1, y - 1);
            //Row 2
            AddNeighbor(currentNeighbors, x - 1, y);
            AddNeighbor(currentNeighbors, x, y);
            AddNeighbor(currentNeighbors, x + 1, y);
            //Row 3
            AddNeighbor(currentNeighbors, x - 1, y + 1);
            AddNeighbor(currentNeighbors, x, y + 1);
            AddNeighbor(currentNeighbors, x + 1, y + 1);
        }

    }
}
