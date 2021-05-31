using LifeSim.Engine2D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LifeSim.UI.CellGraphics
{
    public interface IRenderer<T>
    {
        void Initialize(T surface);
        void Render(CellCollection cellCollection, double cellSize, double xOffset, double yOffset);
    }

}
