using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingAnnihilationSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var eff = new HangingAnnihilationEffect(serverGame: ServerGame,
                                                    triggerRestriction: triggerRestriction,
                                                    endCondition: endCondition,
                                                    fallOffCondition: fallOffCondition,
                                                    fallOffRestriction: CreateFallOffRestriction(Target),
                                                    sourceEff: Effect,
                                                    currentContext: Context,
                                                    target: Target);
            return new List<HangingEffect>() { eff };
        }
    }
}