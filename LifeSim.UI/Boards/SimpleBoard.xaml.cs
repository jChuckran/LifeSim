using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using LifeSim.Engine2D.Models;
using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;

namespace LifeSim.UI.Boards
{
    /// <summary>
    /// Interaction logic for SimpleBoard.xaml
    /// </summary>
    public partial class SimpleBoard : UserControl
    {
        public SimpleBoard()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Generate();
        }

        #region CellSize Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.RegisterAttached(
                "CellSize", typeof(int), typeof(SimpleBoard),
                new PropertyMetadata(25));

        public int CellSize
        {
            get { return (int)GetValue(CellSizeProperty); }
            set { SetValue(CellSizeProperty, value); }
        }

        // Get
        public static int GetCellSize(DependencyObject obj)
        {
            return (int)obj.GetValue(CellSizeProperty);
        }

        // Set
        public static void SetCellSize(DependencyObject obj, int value)
        {
            obj.SetValue(CellSizeProperty, value);
        }

        #endregion

        #region LiveDensity Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty LiveDensityProperty =
            DependencyProperty.RegisterAttached(
                "LiveDensity", typeof(decimal), typeof(SimpleBoard),
                new PropertyMetadata((decimal)0.3));

        public decimal LiveDensity
        {
            get { return (decimal)GetValue(LiveDensityProperty); }
            set { SetValue(LiveDensityProperty, value); }
        }

        // Get
        public static decimal GetLiveDensity(DependencyObject obj)
        {
            return (decimal)obj.GetValue(LiveDensityProperty);
        }

        // Set
        public static void SetLiveDensity(DependencyObject obj, decimal value)
        {
            obj.SetValue(LiveDensityProperty, value);
        }

        #endregion

        #region Running Property

        /// <summary>
        /// If the sim is running
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty RunningProperty =
            DependencyProperty.RegisterAttached(
                "Running", typeof(bool), typeof(SimpleBoard),
                new PropertyMetadata(false));

        public bool Running
        {
            get { return (bool)GetValue(RunningProperty); }
            set { SetValue(RunningProperty, value); }
        }

        // Get
        public static bool GetRunning(DependencyObject obj)
        {
            return (bool)obj.GetValue(RunningProperty);
        }

        // Set
        public static void SetRunning(DependencyObject obj, bool value)
        {
            obj.SetValue(RunningProperty, value);
        }

        #endregion


        public double WorldWidth
        {
            get
            {
                return WorldCanvas.ActualWidth;
            }
        }
        public double WorldHeight
        {
            get
            {
                return WorldCanvas.ActualHeight;
            }
        }

        public double CenterX
        {
            get
            {
                return WorldCanvas.ActualWidth / 2;
            }
        }
        public double CenterY
        {
            get
            {
                return WorldCanvas.ActualHeight / 2;
            }
        }
        public double LeftRenderEdge
        {
            get
            {
                return -(CenterX + CellSize) - XOffset;
            }
        }
        public double TopRenderEdge
        {
            get
            {
                return -(CenterY + CellSize) - YOffset;
            }
        }
        public double RightRenderEdge
        {
            get
            {
                return CenterX + CellSize - XOffset;
            }
        }
        public double BottomRenderEdge
        {
            get
            {
                return CenterY + CellSize - YOffset;
            }
        }

        public double XOffset { get; set; } = 0;
        public double YOffset { get; set; } = 0;

        private CellCollection cellCollection { get; set; } = new CellCollection();

        public async void Generate()
        {
            var start = DateTime.Now;

            cellCollection.ClearCells();

            cellCollection.Randomize(decimal.ToDouble(LiveDensity), (long)LeftRenderEdge / CellSize, (long)RightRenderEdge / CellSize, (long)TopRenderEdge / CellSize, (long)BottomRenderEdge / CellSize);

            //Cells.GetOrAddCell(-1, 0, true, true);
            //Cells.GetOrAddCell(0, 0, true, true);
            //Cells.GetOrAddCell(1, 0, true, true);

            RenderWorld();

            FrameRenderTime.Content = $"{(int)DateTime.Now.Subtract(start).TotalMilliseconds}ms";

            await Task.CompletedTask;
        }

        private bool _runningLoop = false;
        public async void ToggleRunningSim()
        {
            if (Running)
            {
                Running = false;
            }
            else
            {
                while (_runningLoop)
                {
                    await Task.Delay(1);
                }
                Running = true;
                _runningLoop = true;
                while (Running)
                {
                    await Advance();
                }
                _runningLoop = false;
            }
        }

        public async Task Advance(int count = 1)
        {
            for (int x = 0; x < count; x++)
            {
                var start = DateTime.Now;
                await cellCollection.Advance();
                RenderWorld();
                FrameRenderTime.Content = $"{(int)DateTime.Now.Subtract(start).TotalMilliseconds}ms";
            }
        }

        public void Refresh()
        {
            var start = DateTime.Now;
            RenderWorld();
            FrameRenderTime.Content = $"{(int)DateTime.Now.Subtract(start).TotalMilliseconds}ms";
        }

        private void RenderWorld()
        {

            WorldCanvas.Children.Clear();

            LeftEdge.Content = LeftRenderEdge.ToString();
            TopEdge.Content = TopRenderEdge.ToString();
            RightEdge.Content = RightRenderEdge.ToString();
            BottomEdge.Content = BottomRenderEdge.ToString();
            Offsets.Content = $"XOffset: {XOffset.ToString().PadRight(6)} YOffset: {YOffset}";
            Viewable.Content = $"Width: {Math.Abs(LeftRenderEdge - RightRenderEdge)} Height: {Math.Abs(TopRenderEdge - BottomRenderEdge)}";
            Iteration.Content = $"Iteration: {cellCollection?.Iteration}";

            if (cellCollection == null)
                return;

            var viewableCells = cellCollection.Cells.Where((c) => (c.X >= LeftRenderEdge / CellSize && c.X <= RightRenderEdge / CellSize) && (c.Y >= TopRenderEdge / CellSize && c.Y <= BottomRenderEdge / CellSize)).ToList();

            foreach (TrackedCell cell in viewableCells)
            {
                if (cell.IsAlive)
                    AddPoint(CellSize, CellSize, (cell.X * CellSize) + CenterX + XOffset, (cell.Y * CellSize) + CenterY + YOffset, Colors.RoyalBlue);
                //else
                //    AddPoint(CellSize, CellSize, (cell.X * CellSize) + CenterX + XOffset, (cell.Y * CellSize) + CenterY + YOffset, Colors.LightBlue);
            }

            WorldCanvas.UpdateLayout();
        }

        private void AddPoint(double width, double height, double drawX, double drawY, Color color)
        {
            var ellipse = new Ellipse() { Width = width, Height = height };
            ellipse.Fill = new SolidColorBrush(color);
            Canvas.SetLeft(ellipse, drawX - width / 2);
            Canvas.SetTop(ellipse, drawY - height / 2);
            WorldCanvas.Children.Add(ellipse);
        }

        private Point GetCellPoint(Point canvasPoint)
        {
            var cx = Math.Round(canvasPoint.X / CellSize) * CellSize;
            var cy = Math.Round(canvasPoint.Y / CellSize) * CellSize;
            return new Point((cx + XOffset - CenterX) / CellSize, (cy + YOffset - CenterY) / CellSize);
        }

        private void ToggleCellAtCanvasPoint(Point canvasPoint)
        {
            var start = DateTime.Now;
            var cellPoint = GetCellPoint(canvasPoint);
            cellCollection.ToggleCell((long)cellPoint.X, (long)cellPoint.Y);
            RenderWorld();
            FrameRenderTime.Content = $"{(int)DateTime.Now.Subtract(start).TotalMilliseconds}ms";
        }
        
        private void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "LifeSim files (*.lifesim)|*.lifesim" };
            if (saveFileDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var cellJson = cellCollection.Export();
                File.WriteAllText(saveFileDialog.FileName, cellJson);
                Mouse.OverrideCursor = null;
            }
        }

        private void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var cellJson = File.ReadAllText(openFileDialog.FileName);
                cellCollection.Import(cellJson);
                Refresh();
                Mouse.OverrideCursor = null;
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            Generate();
        }

        private void Advance_Click(object sender, RoutedEventArgs e)
        {
            _ = Advance();
        }
        private void AdvanceTen_Click(object sender, RoutedEventArgs e)
        {
            _ = Advance(10);
        }
        private void AdvanceHundred_Click(object sender, RoutedEventArgs e)
        {
            _ = Advance(100);
        }

        private void WorldCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RenderWorld();
        }

        private void CellSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RenderWorld();
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleRunningSim();
        }

        #region Viewport Interaction

        private Point _dragStartPoint;

        private void WorldCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (WorldCanvas.IsMouseCaptured)
            {
                var curPos = e.GetPosition(this);
                XOffset += curPos.X - _dragStartPoint.X;
                YOffset += curPos.Y - _dragStartPoint.Y;
                _dragStartPoint = curPos;
                RenderWorld();
            }
        }

        private void WorldCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                ToggleCellAtCanvasPoint(e.GetPosition(WorldCanvas));
            }
            else
            {
                if (e.ClickCount == 1)
                {
                    _dragStartPoint = e.GetPosition(this);
                    WorldCanvas.Focus();
                    WorldCanvas.CaptureMouse();
                }
            }
            e.Handled = true;
        }

        private void WorldCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            WorldCanvas.ReleaseMouseCapture();
        }

        private void WorldCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var newSize = CellSize + (e.Delta / 120);
            if (newSize < 1)
                newSize = 1; 
            CellSize = newSize;
        }


        #endregion

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            var start = DateTime.Now;
            cellCollection.ClearCells();
            RenderWorld();
            FrameRenderTime.Content = $"{(int)DateTime.Now.Subtract(start).TotalMilliseconds}ms";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
