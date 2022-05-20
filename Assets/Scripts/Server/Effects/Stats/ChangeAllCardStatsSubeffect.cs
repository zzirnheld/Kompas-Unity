using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ChangeAllCardStatsSubeffect : ChangeCardStatsSubeffect
    {
        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public IActivationContextIdentity<ICollection<GameCardBase>> cardsIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            if (cardsIdentity == default)
            {
                cardRestriction ??= new CardRestriction()
                {
                    cardRestrictions = new string[]
                    {
                    CardRestriction.Character,
                    CardRestriction.Board
                    }
                };
                cardRestriction.Initialize(DefaultRestrictionContext);
            }
            else cardsIdentity.Initialize(DefaultRestrictionContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            IEnumerable<GameCard> targetsEnumerable = cardsIdentity == default
                ? ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, CurrentContext))
                : cardsIdentity.From(CurrentContext, default).Select(c => c.Card);
            var targets = targetsEnumerable.ToArray();

                
            if (targets.Length == 0) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            var buff = Buff;
            foreach (var c in targets) c.AddToStats(buff, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}