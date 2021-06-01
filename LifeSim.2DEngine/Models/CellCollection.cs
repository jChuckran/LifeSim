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

        private object cellSync = new object();
        public List<TrackedCell> Cells { get; set; } = new List<TrackedCell>();

        public IRules Rules { get; set; } = new ConwaysGameOfLife();

        public long Iteration { get; set; }

        public string Seed { get; set; } = string.Empty;

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
            Seed = cc.Seed;
            foreach (Cell cell in cc.Cells)
            {
                UpdateCell(cell.X, cell.Y, cell.IsAlive);
            }
        }

        public void GenerateSeed()
        {
            Seed = GenerateSeed(Cells);
        }

        public string GenerateSeed(List<TrackedCell> cells, char deadChar = '.', char aliveChar = 'O')
        {
            var aliveCells = cells.Where(c => c.IsAlive);
            if (!aliveCells.Any())
                return string.Empty;
            var leftMostCell = aliveCells.Aggregate((curMin, x) => (curMin == null || x.X < curMin.X ? x : curMin));
            var topMostCell = aliveCells.Aggregate((curMin, y) => (curMin == null || y.Y < curMin.Y ? y : curMin));
            var rightMostCell = aliveCells.Aggregate((curMax, x) => (curMax == null || x.X > curMax.X ? x : curMax));
            var bottomMostCell = aliveCells.Aggregate((curMax, y) => (curMax == null || y.Y > curMax.Y ? y : curMax));
            var lines = new List<string>();
            for (long y = topMostCell.Y; y <= bottomMostCell.Y; y++) 
            {
                string line = string.Empty;
                for (long x = leftMostCell.X; x <= rightMostCell.X; x++)
                {
                    if (aliveCells.Any(c => c.X == x && c.Y == y))
                        line += aliveChar;
                    else
                        line += deadChar;
                }
                lines.Add(line.TrimEnd(deadChar));
            }
            return lines.Aggregate((a, b) => a + Environment.NewLine + b); ;
        }

        public void ImportSeed(string seed, char aliveChar = 'O')
        {
            lock (cellSync)
            {
                Cells.Clear();
                string[] lines = seed.Split(Environment.NewLine, StringSplitOptions.None);
                var longestLine = lines.Aggregate((longest, l) => !l.StartsWith("!") && (longest == null || l.Length > longest.Length) ? l : longest);
                if (longestLine != null && longestLine.Length == 0)
                {
                    var xOffset = (int)-Math.Floor((double)longestLine.Length / 2);
                    long y = xOffset;
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("!"))
                            continue;
                        for (int x = 0; x < line.Length; x++)
                        {
                            if (line[x] == aliveChar)
                            {
                                UpdateCell(x + xOffset, y, true);
                            }
                        }
                        y++;
                    }
                }
                Iteration = 0;
                Seed = seed;
            }
        }

        public void ClearCells()
        {
            lock (cellSync)
            {
                Cells.Clear();
                Iteration = 0;
                Seed = string.Empty;
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
                Iteration = 0;
                GenerateSeed();
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
