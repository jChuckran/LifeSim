using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeSim.UI.Controls
{
    public class DynamicCell : Control
    {

        static DynamicCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicCell), new FrameworkPropertyMetadata(typeof(DynamicCell)));
        }

        #region X Property

        /// <summary>
        /// Adds the specified number of Rows to RowDefinitions. 
        /// Default Height is Auto
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached(
                "X", typeof(int), typeof(DynamicCell),
                new PropertyMetadata(-1));

        public int X
        {
            get { return (int)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        // Get
        public static int GetX(DependencyObject obj)
        {
            return (int)obj.GetValue(XProperty);
        }

        // Set
        public static void SetX(DependencyObject obj, int value)
        {
            obj.SetValue(XProperty, value);
        }

        #endregion

        #region Y Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.RegisterAttached(
                "Y", typeof(int), typeof(DynamicCell),
                new PropertyMetadata(-1));

        public int Y
        {
            get { return (int)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        // Get
        public static int GetY(DependencyObject obj)
        {
            return (int)obj.GetValue(YProperty);
        }

        // Set
        public static void SetY(DependencyObject obj, int value)
        {
            obj.SetValue(YProperty, value);
        }

        #endregion

    }
}
