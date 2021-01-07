using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;

namespace KompasCore.Networking
{
    public class CardTargetPacket : Packet
    {
        public int cardId;

        public CardTargetPacket() : base(CardTargetChosen) { }

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
        private ServerGame serverGame;

        public GameCard Target => serverGame?.GetCardWithID(cardId);

        public void Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            this.serverGame = serverGame;
            awaiter.EnqueuePacket(this);
            /*
            var currSubeff = serverGame.CurrEffect?.CurrSubeffect;
            var card = serverGame.GetCardWithID(cardId);

            UnityEngine.Debug.Log($"Attempting to target {card?.CardName} in subeffect {currSubeff}");

            if (currSubeff is CardTargetSubeffect cardTargetSubeffect)
                cardTargetSubeffect.AddTargetIfLegal(card);
            else if (currSubeff is ChooseFromListSubeffect listSubeffect)
                listSubeffect.AddListIfLegal(new GameCard[] { card });*/
        }
    }
}