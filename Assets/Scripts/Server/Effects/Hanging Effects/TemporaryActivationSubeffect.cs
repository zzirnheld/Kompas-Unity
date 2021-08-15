using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryActivationSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var tempActivation = new HangingActivationEffect(serverGame: ServerGame,
                                                             triggerRestriction: triggerRestriction,
                                                             endCondition: endCondition,
                                                             fallOffCondition: fallOffCondition,
                                                             fallOffRestriction: CreateFallOffRestriction(Target),
                                                             currentContext: Effect.CurrActivationContext,
                                                             target: Target,
                                                             source: this);
            return new List<HangingEffect>() { tempActivation };
        }
    }
}