using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingDiscardSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var eff = new HangingDiscardEffect(serverGame: ServerGame,
                                               triggerRestriction: triggerRestriction,
                                               endCondition: endCondition,
                                               fallOffCondition: fallOffCondition,
                                               sourceEff: Effect,
                                               fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                               currentContext: Context,
                                               target: CardTarget);
            return new List<HangingEffect>() { eff };
        }
    }
}