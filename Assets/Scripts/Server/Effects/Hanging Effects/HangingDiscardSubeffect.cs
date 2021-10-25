using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingDiscardSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var eff = new HangingDiscardEffect(serverGame: ServerGame,
                                               triggerRestriction: triggerRestriction,
                                               endCondition: endCondition,
                                               fallOffCondition: fallOffCondition,
                                               sourceEff: Effect,
                                               fallOffRestriction: CreateFallOffRestriction(Target),
                                               currentContext: Context,
                                               target: Target);
            return new List<HangingEffect>() { eff };
        }
    }
}