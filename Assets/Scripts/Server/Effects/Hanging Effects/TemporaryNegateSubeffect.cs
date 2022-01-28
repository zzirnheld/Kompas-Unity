using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryNegateSubeffect : HangingEffectSubeffect
    {
        public bool negated = true;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = Context.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets);
            var tempNegation = new HangingNegationEffect(serverGame: ServerGame,
                                                         triggerRestriction: triggerRestriction,
                                                         endCondition: endCondition,
                                                         fallOffCondition: fallOffCondition,
                                                         fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                         currentContext: contextCopy,
                                                         target: CardTarget,
                                                         source: this,
                                                         negated: negated);
            return new List<HangingEffect>() { tempNegation };
        }
    }
}