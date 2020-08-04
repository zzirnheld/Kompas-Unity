using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;

namespace KompasCore.Networking
{
    public class CardTargetPacket : Packet
    {
        public int cardId;

        public CardTargetPacket() : base(CardTarget) { }

        public CardTargetPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new CardTargetPacket(cardId);
    }
}

namespace KompasServer.Networking
{
    public class CardTargetServerPacket : CardTargetPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var currSubeff = serverGame.CurrEffect?.CurrSubeffect;
            var card = serverGame.GetCardWithID(cardId);

            if (currSubeff is CardTargetSubeffect cardTargetSubeffect)
                cardTargetSubeffect.AddTargetIfLegal(card);
            else if (currSubeff is ChooseFromListSubeffect listSubeffect)
                listSubeffect.AddListIfLegal(new GameCard[] { card });
        }
    }
}