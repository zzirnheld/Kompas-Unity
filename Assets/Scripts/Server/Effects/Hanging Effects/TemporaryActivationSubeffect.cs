using System.Collections.Generic;
using KompasCore.Cards;

namespace KompasServer.Effects
{
    public class TemporaryActivationSubeffect : TemporaryCardChangeSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var tempActivation = new HangingActivationEffect(ServerGame, triggerRestriction, endCondition,
                fallOffCondition, CreateFallOffRestriction(Target),
                Target, this);
            return new List<HangingEffect>() { tempActivation };
        }
    }
}