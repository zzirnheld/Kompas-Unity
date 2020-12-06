using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Linq;
using System.Collections.Generic;

namespace KompasCore.Networking
{
    public class ListChoicesPacket : Packet
    {
        public int[] cardIds;

        public ListChoicesPacket() : base(ListChoicesChosen) { }

        public ListChoicesPacket(int[] cardIds) : this()
        {
            this.cardIds = cardIds;
        }

        public ListChoicesPacket(IEnumerable<GameCard> cards) : this(cards.Select(c => c.ID).ToArray()) { }

        public override Packet Copy() => new ListChoicesPacket(cardIds);
    }
}

namespace KompasServer.Networking
{
    public class ListChoicesServerPacket : ListChoicesPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var choices = cardIds.Select(c => serverGame.GetCardWithID(c)).Where(c => c != null).Distinct();

            var currSubeff = serverGame.CurrEffect?.CurrSubeffect;
            if (currSubeff is ChooseFromListSubeffect listEff)
                listEff.AddListIfLegal(choices);
            else if (currSubeff is CardTargetSubeffect deckTargetSubeffect)
                deckTargetSubeffect.AddTargetIfLegal(choices.FirstOrDefault());
        }
    }
}