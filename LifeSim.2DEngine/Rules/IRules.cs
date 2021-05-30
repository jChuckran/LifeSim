using LifeSim.Engine2D.Models;

namespace LifeSim.Engine2D.Rules
{
    public interface IRules
    {
        public bool GetNextLivingState(TrackedCell cell);
    }
}
