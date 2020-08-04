using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Linq;

namespace KompasCore.Networking
{
    public class ListChoicesPacket : Packet
    {
        public int[] cardIds;

        public ListChoicesPacket() : base(ListChoicesPacket) { }

        public ListChoicesPacket(int[] cardIds) : this()
        {
            this.cardIds = cardIds;
        }

        public override Packet Copy() => new ListChoicesPacket(cardIds);
    }
}

namespace KompasServer.Networking
{
    public class ListChoicesServerPacket : ListChoicesPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var choices = cardIds.Select(c => serverGame.GetCardWithID(c)).Where(c => c != null);

            var currSubeff = serverGame.CurrEffect?.CurrSubeffect;
            if (currSubeff is ChooseFromListSubeffect listEff)
                listEff.AddListIfLegal(choices);
            else if (currSubeff is DeckTargetSubeffect deckTargetSubeffect)
                deckTargetSubeffect.AddTargetIfLegal(choices.FirstOrDefault());
        }
    }
}