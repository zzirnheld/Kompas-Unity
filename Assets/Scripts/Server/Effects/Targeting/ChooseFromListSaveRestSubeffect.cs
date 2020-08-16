using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects
{
    public class ChooseFromListSaveRestSubeffect : ChooseFromListSubeffect
    {
        protected override void AddList(IEnumerable<GameCard> choices)
        {
            base.AddList(choices);
            var others = potentialTargets.Except(choices);
            ServerEffect.Rest.AddRange(others);
            ServerEffect.X = others.Count();
        }
    }
}