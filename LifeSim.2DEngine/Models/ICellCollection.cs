using LifeSim.Engine2D.Rules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeSim.Engine2D.Models
{
    public interface ICellCollection
    {
        List<TrackedCell> Cells { get; set; }
        IRules Rules { get; set; }
        long Iteration { get; set; }
        void AddExisting(List<TrackedCell> existing, long x, long y);
        void AddLivingCell(long x, long y);
        Task Advance();
        void ClearCells();
        string Export();
        void GetNewNeighbors(List<TrackedCell> currentNeighbors, long x, long y);
        TrackedCell GetOrAddCell(long x, long y, bool newIsAlive, bool updateExisting = false);
        void Import(string cellJson);
        void Randomize(double liveDensity, long startX, long endX, long startY, long endY);
        void ToggleCell(long x, long y);
        void UpdateCell(long x, long y, bool isAlive);
    }
}