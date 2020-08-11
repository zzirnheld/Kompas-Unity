using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryNegateSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var tempNegation = new HangingNegationEffect(ServerGame, triggerRestriction, endCondition, 
                fallOffCondition, CreateFallOffRestriction(Target),
                Target, this);
            return new List<HangingEffect>() { tempNegation };
        }
    }
}