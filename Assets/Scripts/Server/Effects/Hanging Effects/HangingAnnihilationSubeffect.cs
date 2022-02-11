using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingAnnihilationSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = Context.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets);
            var eff = new HangingAnnihilationEffect(serverGame: ServerGame,
                                                    triggerRestriction: triggerRestriction,
                                                    endCondition: endCondition,
                                                    fallOffCondition: fallOffCondition,
                                                    fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                    sourceEff: Effect,
                                                    currentContext: contextCopy,
                                                    target: CardTarget);
            return new List<HangingEffect>() { eff };
        }
    }
}