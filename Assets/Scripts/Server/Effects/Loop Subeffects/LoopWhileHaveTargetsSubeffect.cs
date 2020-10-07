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
                //if we're deleting and there's something to delete, delete it.
                if (delete && ServerEffect.Targets.Any()) RemoveTarget();
                //after any delete that might have happened, check if there's still targets
                return ServerEffect.Targets.Any();
            }
        }
    }
}