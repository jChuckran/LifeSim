using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace LifeSim.UI.Controls
{
    /// <summary>
    /// Interaction logic for DynamicGrid.xaml
    /// </summary>
    public partial class DynamicGrid : UserControl
    {
        public DynamicGrid()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region RowCount Property

        /// <summary>
        /// Adds the specified number of Rows to RowDefinitions. 
        /// Default Height is Auto
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(-1));

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        // Get
        public static int GetRowCount(DependencyObject obj)
        {
            return (int)obj.GetValue(RowCountProperty);
        }

        // Set
        public static void SetRowCount(DependencyObject obj, int value)
        {
            obj.SetValue(RowCountProperty, value);
        }

        #endregion

        #region ColumnCount Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(-1));

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        // Get
        public static int GetColumnCount(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnCountProperty);
        }

        // Set
        public static void SetColumnCount(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnCountProperty, value);
        }

        #endregion.

        public async Task ClearCells()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                foreach (DynamicCell cell in MainGrid.Children)
                {
                    cell.DataContext = null;
                }    
                MainGrid.Children.Clear();
            });
        }

        public async Task AddCell(int x, int y, object context)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                DynamicCell dynamicCell = new DynamicCell();
                dynamicCell.X = x;
                dynamicCell.Y = y;
                dynamicCell.DataContext = context;
                MainGrid.Children.Add(dynamicCell);
                dynamicCell.SetValue(Grid.ColumnProperty, x);
                dynamicCell.SetValue(Grid.RowProperty, y);
            });
        }

    }
}
