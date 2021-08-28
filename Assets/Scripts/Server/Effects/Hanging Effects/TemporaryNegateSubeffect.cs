using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryNegateSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var tempNegation = new HangingNegationEffect(serverGame: ServerGame,
                                                         triggerRestriction: triggerRestriction,
                                                         endCondition: endCondition,
                                                         fallOffCondition: fallOffCondition,
                                                         fallOffRestriction: CreateFallOffRestriction(Target),
                                                         currentContext: Context,
                                                         target: Target,
                                                         source: this);
            return new List<HangingEffect>() { tempNegation };
        }
    }
}