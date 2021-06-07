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

        public SimpleCanvasRenderer(Canvas surface) : base(surface)
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
