using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryActivationSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var tempActivation = new HangingActivationEffect(serverGame: ServerGame,
                                                             triggerRestriction: triggerRestriction,
                                                             endCondition: endCondition,
                                                             fallOffCondition: fallOffCondition,
                                                             fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                             sourceEff: Effect,
                                                             currentContext: Context,
                                                             target: CardTarget,
                                                             source: this);
            return new List<HangingEffect>() { tempActivation };
        }
    }
}