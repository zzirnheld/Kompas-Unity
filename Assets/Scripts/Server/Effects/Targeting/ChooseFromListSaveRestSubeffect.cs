using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class ChooseFromListSaveRestSubeffect : ChooseFromListSubeffect
    {
        public CardRestriction restRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            if (restRestriction == null) restRestriction = cardRestriction;
            else restRestriction.Initialize(this);
        }

        protected override bool NoPossibleTargets()
        {
            var rest = ServerGame.Cards.Where(c => restRestriction.Evaluate(c));
            ServerEffect.rest.AddRange(rest);
            return base.NoPossibleTargets();
        }

        protected override void AddList(IEnumerable<GameCard> choices)
        {
            base.AddList(choices);
            var rest = ServerGame.Cards.Where(c => restRestriction.Evaluate(c) && !choices.Contains(c));
            ServerEffect.rest.AddRange(rest);
        }
    }
}