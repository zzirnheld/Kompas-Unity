using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryNegateSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var tempNegation = new HangingNegationEffect(serverGame: ServerGame,
                                                         triggerRestriction: triggerRestriction,
                                                         endCondition: endCondition,
                                                         fallOffCondition: fallOffCondition,
                                                         fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                         currentContext: Context,
                                                         target: CardTarget,
                                                         source: this);
            return new List<HangingEffect>() { tempNegation };
        }
    }
}