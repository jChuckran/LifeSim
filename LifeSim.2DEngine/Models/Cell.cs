using LifeSim.Engine2D.Helpers;

namespace LifeSim.Engine2D.Models
{
    public class Cell : INotifyPropertyChangedEx, ICell
    {
        private bool _isAlive;
        public bool IsAlive
        {
            get { return _isAlive; }
            set
            {
                _isAlive = value;
                OnPropertyChanged();
            }
        }

        public long X { get; set; }
        public long Y { get; set; }

    }
}
