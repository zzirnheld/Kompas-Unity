using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAllCardsIdentity : ServerSubeffect
    {
        public IActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cardsIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardsIdentity.Initialize(DefaultInitializationContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var cards = cardsIdentity.From(CurrentContext, CurrentContext);
            if (cards.Count == 0) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            var shuffledCards = CollectionsHelper.Shuffle<GameCardBase>(cards);
            foreach (var card in shuffledCards)
            {
                if (card == null) continue;
                Effect.AddTarget(card.Card);
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}