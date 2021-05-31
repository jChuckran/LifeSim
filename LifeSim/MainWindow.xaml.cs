using System.Windows;

namespace LifeSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
