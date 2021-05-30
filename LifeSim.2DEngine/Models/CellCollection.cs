using LifeSim.Engine2D.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LifeSim.Engine2D.Rules;

namespace LifeSim.Engine2D.Models
{
    public class CellCollection : ICellCollection
    {

        public List<TrackedCell> Cells { get; set; } = new List<TrackedCell>();

        public IRules Rules { get; set; } = new ConwaysGameOfLife();

        private object cellSync = new object();

        public long Iteration { get; set; }

        public string Export()
        {
            var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto};
            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }

        public void Import(string cellJson)
        {
            ClearCells();
            var cc = JsonConvert.DeserializeObject<CellCollection>(cellJson);
            Rules = cc.Rules;
            Iteration = cc.Iteration;
            foreach (Cell cell in cc.Cells)
            {
                UpdateCell(cell.X, cell.Y, cell.IsAlive);
            }
        }

        public void ClearCells()
        {
            lock (cellSync)
            {
                Cells.Clear();
                Iteration = 0;
            }
        }

        public async Task Advance()
        {
            await Task.Run(() =>
            {
                lock (cellSync)
                {
                    var allCells = Cells.ToList();
                    foreach (TrackedCell cell in allCells)
                    {
                        cell.DetermineNextState(Rules);
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
                            Cells.Remove(cell);
                        }
                    }
                    Iteration++;
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
            var res = Cells.Where((c) => c.X == x && c.Y == y);
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
                var res = Cells.Where((c) => c.X == x && c.Y == y);
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
            Cells.Add(newCell);
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
