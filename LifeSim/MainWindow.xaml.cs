using System.Windows;
using MahApps.Metro.Controls;

namespace LifeSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Title = $"LifeSim | Board Width:{Board.WorldWidth:N0} Board Height:{Board.WorldHeight:N0}";
        }
    }
}
