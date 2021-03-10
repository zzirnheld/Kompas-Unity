using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingDiscardSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var eff = new HangingDiscardEffect(ServerGame, triggerRestriction, endCondition, 
                fallOffCondition, CreateFallOffRestriction(Target),
                Target);
            return new List<HangingEffect>() { eff };
        }
    }
}