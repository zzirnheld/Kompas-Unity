using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetListChoicesPacket : Packet
    {
        public int[] cardIds;
        public int max;
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetListChoicesPacket() : base(GetListChoices) { }

        public GetListChoicesPacket(int[] cardIds, int max, int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.cardIds = cardIds;
            this.max = max;
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public override Packet Copy() => new GetListChoicesPacket(cardIds, max, sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetListChoicesClientPacket : GetListChoicesPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var cards = cardIds.Select(i => clientGame.GetCardWithID(i)).Where(c => c != null);
            var source = clientGame.GetCardWithID(sourceCardId);
            if (source == null) return;
            var subeff = source.Effects.ElementAt(effIndex).Subeffects[subeffIndex] as DummyListTargetSubeffect;
            var listRestriction = subeff?.listRestriction;
            clientGame.CurrCardRestriction = subeff?.cardRestriction;
            clientGame.targetMode = Game.TargetMode.OnHold;
            clientGame.clientUICtrl.StartSearch(cards.ToList(), max);
            clientGame.clientUICtrl.SetCurrState($"Choose Target for Effect of {source.CardName}",
                clientGame.CurrCardRestriction?.blurb);

            //can't just do foreach( in cards) because also need to turn off any remaining targets that aren't in the list
            clientGame.ShowValidCardTargets(c => cards.Contains(c));
        }
    }
}