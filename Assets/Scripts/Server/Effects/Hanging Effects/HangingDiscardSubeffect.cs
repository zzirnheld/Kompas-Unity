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
                                               fallOffRestriction: CreateFallOffRestriction(Target),
                                               currentContext: Effect.CurrActivationContext,
                                               target: Target);
            return new List<HangingEffect>() { eff };
        }
    }
}