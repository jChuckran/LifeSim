using LifeSim.Engine2D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace LifeSim.UI.CellGraphics
{
    public class AgeCanvasRenderer : CanvasRendererBase
    {

        #region Properties

        private bool _showAllCells = false;
        public bool ShowAllCells
        {
            get { return _showAllCells; }
            set
            {
                _showAllCells = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public AgeCanvasRenderer(Canvas surface) : base(surface)
        {
        }

        public override void Render(CellCollection cellCollection, double cellSize, double xOffset, double yOffset)
        {
            Surface.Children.Clear();
            var edges = GetRenderEdges(cellSize, xOffset, yOffset);
            var viewableCells = GetVisibleCells(cellCollection, cellSize, edges);
            //Draw Grid Lines
            if (ShowGridLines)
            {
                RenderGridLines(cellSize, xOffset, yOffset, edges);
            }
            //Draw Cells
            var size = cellSize * 0.9;
            foreach (TrackedCell cell in viewableCells)
            {            
                if (cell.IsAlive)
                {
                    if (cell.Age == 0)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.LightSkyBlue);
                    }
                    else if (cell.Age == 1)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.Chartreuse);
                    }
                    else if (cell.Age == 2)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.BlueViolet);
                    }
                    else if (cell.Age == 3)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.Red);
                    }
                    else if (cell.Age == 4)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.Blue);
                    }
                    else if (cell.Age == 5)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.Coral);
                    }
                    else if (cell.Age >= 6)
                    {
                        AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.Gold);
                    }
                }
                else if (ShowAllCells)
                    AddEllipse(size, size, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, Colors.Black);
            }
        }

    }
}
