using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LifeSim.UI.Helpers
{
    public class GridHelpers
    {
        #region RowCount Property

        /// <summary>
        /// Adds the specified number of Rows to RowDefinitions. 
        /// Default Height is Auto
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount", typeof(int), typeof(GridHelpers),
                new PropertyMetadata(-1, RowCountChanged));

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

        // Change Event - Adds the Rows
        public static void RowCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || (int)e.NewValue < 0)
                return;
            Grid grid = (Grid)obj;
            UpdateRows(grid, (int)e.NewValue);
        }

        #endregion

        #region ColumnCount Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount", typeof(int), typeof(GridHelpers),
                new PropertyMetadata(-1, ColumnCountChanged));

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

        // Change Event - Add the Columns
        public static void ColumnCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || (int)e.NewValue < 0)
                return;
            Grid grid = (Grid)obj;
            UpdateColumns(grid, (int)e.NewValue);
        }

        #endregion

        #region ColumnWidth Property

        /// <summary>
        /// Specifies the width of columns
        /// Default is 1*
        /// </summary>
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.RegisterAttached(
                "ColumnWidth", typeof(GridLength), typeof(GridHelpers),
                new PropertyMetadata(new GridLength(1, GridUnitType.Star), ColumnWidthChanged));

        // Get
        public static GridLength GetColumnWidth(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(ColumnWidthProperty);
        }

        // Set
        public static void SetColumnWidth(DependencyObject obj, GridLength value)
        {
            obj.SetValue(ColumnWidthProperty, value);
        }

        // Change Event - Add the Columns
        public static void ColumnWidthChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid))
                return;
            Grid grid = (Grid)obj;
            UpdateColumns(grid, (GridLength)e.NewValue);
        }

        #endregion

        #region RowHeight Property

        /// <summary>
        /// Specifies the height of rows
        /// Default is 1*
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.RegisterAttached(
                "RowHeight", typeof(GridLength), typeof(GridHelpers),
                new PropertyMetadata(new GridLength(1, GridUnitType.Star), RowHeightChanged));

        // Get
        public static GridLength GetRowHeight(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(RowHeightProperty);
        }

        // Set
        public static void SetRowHeight(DependencyObject obj, GridLength value)
        {
            obj.SetValue(RowHeightProperty, value);
        }

        // Change Event - Add the Columns
        public static void RowHeightChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid))
                return;
            Grid grid = (Grid)obj;
            UpdateRows(grid, (GridLength)e.NewValue);
        }

        #endregion


        #region Update Methods

        public static void UpdateGrid(Grid grid)
        {
            UpdateColumns(grid);
            UpdateRows(grid);
        }

        public static void UpdateColumns(Grid grid)
        {
            UpdateColumns(grid, (int)grid.GetValue(GridHelpers.ColumnCountProperty), (GridLength)grid.GetValue(GridHelpers.ColumnWidthProperty));
        }

        public static void UpdateRows(Grid grid)
        {
            UpdateRows(grid, (int)grid.GetValue(GridHelpers.RowCountProperty), (GridLength)grid.GetValue(GridHelpers.RowHeightProperty));
        }

        public static void UpdateColumns(Grid grid, int count)
        {
            UpdateColumns(grid, count, (GridLength)grid.GetValue(GridHelpers.ColumnWidthProperty));
        }
        public static void UpdateRows(Grid grid, int count)
        {
            UpdateRows(grid, count, (GridLength)grid.GetValue(GridHelpers.RowHeightProperty));
        }
        public static void UpdateColumns(Grid grid, GridLength width)
        {
            UpdateColumns(grid, (int)grid.GetValue(GridHelpers.ColumnCountProperty), width);
        }
        public static void UpdateRows(Grid grid, GridLength height)
        {
            UpdateRows(grid, (int)grid.GetValue(GridHelpers.RowCountProperty), height);
        }

        public static void UpdateColumns(Grid grid, int count, GridLength width)
        {
            grid.ColumnDefinitions.Clear();
            for (int i = 0; i < count; i++)
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = width });
        }

        public static void UpdateRows(Grid grid, int count, GridLength height)
        {
            grid.RowDefinitions.Clear();
            for (int i = 0; i < count; i++)
                grid.RowDefinitions.Add(
                    new RowDefinition() { Height = height });
        }

        #endregion

    }
}
