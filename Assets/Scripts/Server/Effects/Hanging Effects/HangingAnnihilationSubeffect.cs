using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingAnnihilationSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var eff = new HangingAnnihilationEffect(ServerGame, triggerRestriction, endCondition, 
                fallOffCondition, CreateFallOffRestriction(Target),
                Target);
            return new List<HangingEffect>() { eff };
        }
    }
}