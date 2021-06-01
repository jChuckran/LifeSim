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
using LifeSim.UI.CellGraphics;

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
            InitializeRenderer();
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

        #region CellColor Property

        /// <summary>
        /// Sets the cell color 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty CellColorProperty =
            DependencyProperty.RegisterAttached(
                "CellColor", typeof(Color), typeof(SimpleBoard),
                new PropertyMetadata(Colors.RoyalBlue, CellColorChanged));

        public Color CellColor
        {
            get { return (Color)GetValue(CellColorProperty); }
            set { SetValue(CellColorProperty, value); }
        }

        // Get
        public static Color GetCellColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(CellColorProperty);
        }

        // Set
        public static void SetCellColor(DependencyObject obj, Color value)
        {
            obj.SetValue(CellColorProperty, value);
        }

        public static void CellColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is SimpleBoard))
                return;
            SimpleBoard board = (SimpleBoard)obj;
            board.Refresh();
        }

        #endregion

        #region GridLineColor Property

        /// <summary>
        /// Sets the cell color 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty GridLineColorProperty =
            DependencyProperty.RegisterAttached(
                "GridLineColor", typeof(Color), typeof(SimpleBoard),
                new PropertyMetadata(Colors.Black, GridLineColorChanged));

        public Color GridLineColor
        {
            get { return (Color)GetValue(GridLineColorProperty); }
            set { SetValue(GridLineColorProperty, value); }
        }

        // Get
        public static Color GetGridLineColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(GridLineColorProperty);
        }

        // Set
        public static void SetGridLineColor(DependencyObject obj, Color value)
        {
            obj.SetValue(GridLineColorProperty, value);
        }

        public static void GridLineColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is SimpleBoard))
                return;
            SimpleBoard board = (SimpleBoard)obj;
            board.Refresh();
        }

        #endregion

        public CanvasRendererBase Renderer { get; set; }

        private void InitializeRenderer()
        {
            Renderer = new SimpleCanvasRenderer(WorldCanvas);
        }

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

            cellCollection.GenerateSeed();

            RenderWorld();

            UpdateIteration(start);

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
                    await Task.Delay(1);
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
                UpdateIteration(start);
            }
        }

        public void Refresh()
        {
            if (Renderer == null)
                return;
            RenderWorld();
        }

        private void RenderWorld()
        {
            if (Renderer == null)
                return;
            UpdateInfo();
            if (Renderer is SimpleCanvasRenderer scr)
            {
                scr.AliveCellColor = CellColor;
                scr.GridLinesColor = GridLineColor;
            }
            Renderer.Render(cellCollection, CellSize, XOffset, YOffset);
        }

        private void UpdateIteration(DateTime iterationStart)
        {
            Iteration.Content = $"Iteration: {cellCollection.Iteration:N0} ({(int)DateTime.Now.Subtract(iterationStart).TotalMilliseconds}ms)";
        }

        private void UpdateInfo()
        {
            LeftEdge.Content = LeftRenderEdge.ToString("N0");
            TopEdge.Content = TopRenderEdge.ToString("N0");
            RightEdge.Content = RightRenderEdge.ToString("N0");
            BottomEdge.Content = BottomRenderEdge.ToString("N0");
            Offsets.Content = $"XOffset: {XOffset:N0} YOffset: {YOffset:N0}";
            CellCount.Content = $"Total Cells: {cellCollection.Cells.Count():N0} Living Cells: {cellCollection.Cells.Where((c) => c.IsAlive).Count():N0}";
        }

        private Point GetCellPoint(Point canvasPoint)
        {
            var cx = Math.Round((canvasPoint.X - XOffset) / CellSize) * CellSize;
            var cy = Math.Round((canvasPoint.Y - YOffset) / CellSize) * CellSize;
            return new Point((cx - CenterX) / CellSize , (cy - CenterY) / CellSize);
        }

        private void ToggleCellAtCanvasPoint(Point canvasPoint)
        {
            var start = DateTime.Now;
            var cellPoint = GetCellPoint(canvasPoint);
            cellCollection.ToggleCell((long)cellPoint.X, (long)cellPoint.Y);
            RenderWorld();
            UpdateInfo();
            UpdateIteration(start);
        }

        private void ClearCells()
        {
            var start = DateTime.Now;
            cellCollection.ClearCells();
            RenderWorld();
            UpdateInfo();
            UpdateIteration(start);
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
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "LifeSim files (*.lifesim)|*.lifesim" };
            if (openFileDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var cellJson = File.ReadAllText(openFileDialog.FileName);
                cellCollection.Import(cellJson);
                Refresh();
                Mouse.OverrideCursor = null;
            }
            //OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "cell files (*.cell)|*.cell" };
            //if (openFileDialog.ShowDialog() == true)
            //{
            //    Mouse.OverrideCursor = Cursors.Wait;
            //    var cellJson = File.ReadAllText(openFileDialog.FileName);
            //    cellCollection.Import(cellJson);
            //    Refresh();
            //    Mouse.OverrideCursor = null;
            //}
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

        private void WorldCanvas_MouseMove(object sender, MouseEventArgs e)
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

        private void WorldCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void WorldCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WorldCanvas.ReleaseMouseCapture();
        }

        private void WorldCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var newSize = CellSize + (e.Delta / 120);
            if (newSize < 3)
                newSize = 3; 
            CellSize = newSize;
            if (!Running)
                RenderWorld();
        }


        #endregion

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearCells();
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
