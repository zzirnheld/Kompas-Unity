using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasServer.Effects
{
    public class SetXBoardRestrictionSubeffect : SetXSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
        }

        public override int BaseCount => Game.boardCtrl.CardsAndAugsWhere(c => cardRestriction.Evaluate(c)).Count;
    }
}
