using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSim.Engine2D.Models
{
    public interface ICell
    {
        bool IsAlive { get; set; }
        long X { get; set; }
        long Y { get; set; }
    }
}
