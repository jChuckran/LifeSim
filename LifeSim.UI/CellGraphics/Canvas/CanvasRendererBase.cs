using LifeSim.UI.Helpers;
using LifeSim.Engine2D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LifeSim.UI.CellGraphics
{
    public abstract class CanvasRendererBase : INotifyPropertyChangedEx, IRenderer<Canvas>
    {

        public Canvas Surface;

        public double SurfaceWidth
        {
            get
            {
                return Surface.ActualWidth;
            }
        }
        public double SurfaceHeight
        {
            get
            {
                return Surface.ActualHeight;
            }
        }
        public double CenterX
        {
            get
            {
                return Surface.ActualWidth / 2;
            }
        }
        public double CenterY
        {
            get
            {
                return Surface.ActualHeight / 2;
            }
        }

        public Rect GetRenderEdges(double cellSize, double xOffset, double yOffset)
        {
            return new Rect(-(CenterX + cellSize) - xOffset, -(CenterY + cellSize) - yOffset, SurfaceWidth + cellSize * 2, SurfaceHeight + cellSize * 2);
        }

        public List<TrackedCell> GetVisibleCells(CellCollection cellCollection, double cellSize, double xOffset, double yOffset)
        {
            return GetVisibleCells(cellCollection, cellSize, GetRenderEdges(cellSize, xOffset, yOffset));
        }

        public List<TrackedCell> GetVisibleCells(CellCollection cellCollection, double cellSize, Rect renderEdges)
        {
            return cellCollection.Cells.Where((c) => c.X >= renderEdges.Left / cellSize && c.X <= renderEdges.Right / cellSize && c.Y >= renderEdges.Top / cellSize && c.Y <= renderEdges.Bottom / cellSize).ToList();
        }

        public void AddEllipse(double width, double height, double drawX, double drawY, Color color)
        {
            var ellipse = new Ellipse() { Width = width, Height = height };
            ellipse.Fill = new SolidColorBrush(color);
            Canvas.SetLeft(ellipse, drawX - width / 2);
            Canvas.SetTop(ellipse, drawY - height / 2);
            Surface.Children.Add(ellipse);
        }

        public void AddLine(double x1, double y1, double x2, double y2, double width, Color color)
        {
            var line = new Line() { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2 };
            line.Stroke = new SolidColorBrush(color);
            line.StrokeThickness = width;
            Surface.Children.Add(line);
        }

        public void Initialize(Canvas surface)
        {
            Surface = surface;
        }

        public abstract void Render(CellCollection cellCollection, double cellSize, double xOffset, double yOffset);

    }
}
