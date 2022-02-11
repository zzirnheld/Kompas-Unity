using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryActivationSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = Context.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets);
            var tempActivation = new HangingActivationEffect(serverGame: ServerGame,
                                                             triggerRestriction: triggerRestriction,
                                                             endCondition: endCondition,
                                                             fallOffCondition: fallOffCondition,
                                                             fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                             sourceEff: Effect,
                                                             currentContext: contextCopy,
                                                             target: CardTarget,
                                                             source: this);
            return new List<HangingEffect>() { tempActivation };
        }
    }
}