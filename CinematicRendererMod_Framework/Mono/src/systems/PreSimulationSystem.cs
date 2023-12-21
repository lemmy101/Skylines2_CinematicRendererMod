using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game;
using Game.Serialization;
using Game.Simulation;
 
namespace LemmyModFramework.systems
{
    public class PreSimulationSystem : GameSystemBase
    {
        protected override void OnUpdate()
        {
            int gfd = 0;
        }
    }

    public class PostSimulationSystem : GameSystemBase
    {
        protected override void OnUpdate()
        {
            int gfd = 0;
        }
    }
}
