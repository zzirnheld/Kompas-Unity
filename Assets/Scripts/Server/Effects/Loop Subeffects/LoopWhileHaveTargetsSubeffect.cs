using System.Linq;

namespace KompasServer.Effects
{
    public class LoopWhileHaveTargetsSubeffect : LoopSubeffect
    {
        protected override bool ShouldContinueLoop => ServerEffect.Targets.Any();
    }
}