using System.Linq;

namespace KompasServer.Effects
{
    public class LoopWhileHaveTargetsSubeffect : LoopSubeffect
    {
        public bool delete = false;

        protected override bool ShouldContinueLoop
        {
            get
            {
                if (delete && ServerEffect.Targets.Any())
                    ServerEffect.Targets.Remove(ServerEffect.Targets.Last());
                return ServerEffect.Targets.Any();
            }
        }
    }
}