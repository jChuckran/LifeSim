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
    public class SimpleCanvasRenderer : CanvasRendererBase
    {

        #region Properties

        private Color _aliveCellColor = Colors.RoyalBlue;
        public Color AliveCellColor
        {
            get { return _aliveCellColor; }
            set
            {
                _aliveCellColor = value;
                OnPropertyChanged();
            }
        }

        private Color _deadCellColor = Colors.LightBlue;
        public Color DeadCellColor
        {
            get { return _deadCellColor; }
            set
            {
                _deadCellColor = value;
                OnPropertyChanged();
            }
        }

        private Color _gridLinesColor = Colors.Black;
        public Color GridLinesColor
        {
            get { return _gridLinesColor; }
            set
            {
                _gridLinesColor = value;
                OnPropertyChanged();
            }
        }

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

        private bool _showGridLines = true;
        public bool ShowGridLines
        {
            get { return _showGridLines; }
            set
            {
                _showGridLines = value;
                OnPropertyChanged();
            }
        }

        private double _gridLineSize = 3;
        public double GridLineSize
        {
            get { return _gridLineSize; }
            set
            {
                _gridLineSize = value;
                OnPropertyChanged();
            }
        }

        #endregion


        public SimpleCanvasRenderer(Canvas surface)
        {
            Initialize(surface);
        }

        public override void Render(CellCollection cellCollection, double cellSize, double xOffset, double yOffset)
        {
            Surface.Children.Clear();
            var edges = GetRenderEdges(cellSize, xOffset, yOffset);
            var viewableCells = GetVisibleCells(cellCollection, cellSize, edges);
            //Draw Grid Lines
            if (ShowGridLines)
            {
                var columns = edges.Width / cellSize;
                var rows = edges.Height / cellSize;
                var leftMostLine = edges.Left - edges.Left % cellSize;
                var topMostLine = edges.Top - edges.Top % cellSize;
                for (double c = 0; c <= columns; c++)
                {
                    var l = leftMostLine + (c * cellSize);
                    var newLine = l + CenterX + xOffset - (cellSize / 2);
                    AddLine(newLine, 0, newLine, Surface.ActualHeight, 1, GridLinesColor);
                }
                for (double r = 0; r <= rows; r++)
                {
                    var t = topMostLine + (r * cellSize);
                    var newLine = t + CenterY + yOffset - (cellSize / 2);
                    AddLine(0, newLine, Surface.ActualWidth, newLine, 1, GridLinesColor);
                }
            }
            //Draw Cells
            foreach (TrackedCell cell in viewableCells)
            {
                if (cell.IsAlive)
                    AddEllipse(cellSize, cellSize, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, AliveCellColor);
                else if (ShowAllCells)
                    AddEllipse(cellSize, cellSize, (cell.X * cellSize) + CenterX + xOffset, (cell.Y * cellSize) + CenterY + yOffset, DeadCellColor);
            }
        }


    }
}
