using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotallyFair.Graphics
{
    public enum AnimationState
    {
        NONE, 
        IDLE, 
        RUNNINGLEFT, RUNNINGRIGHT, RUNNINGDOWN, RUNNINGUP,
        FALLINGLEFT, FALLINGRIGHT, FALLINGUP, FALLINGDOWN,
        JUMPINGLEFT, JUMPINGRIGHT, JUMPINGUP, JUMPINGDOWN,
        ATTACKINGLEFT, ATTACKINGRIGHT, ATTACKINGUP, ATTACKINGDOWN
    }
}
