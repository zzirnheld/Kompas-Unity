using KompasCore.Networking;
using KompasClient.GameCore;
using UnityEngine;

namespace KompasCore.Networking
{
    public class ChangeCardControllerPacket : Packet
    {
        public int cardId;
        public int controllerIndex;

        public ChangeCardControllerPacket() : base(ChangeCardController) { }

        public ChangeCardControllerPacket(int cardId, int controllerIndex, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
        }

        public override Packet Copy() => new ChangeCardControllerPacket(cardId, controllerIndex);

        public override Packet GetInversion(bool known) => new ChangeCardControllerPacket(cardId, controllerIndex, invert: true);
    }
}

namespace KompasClient.Networking
{
    public class ChangeCardControllerClientPacket : ChangeCardControllerPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            var controller = clientGame.Players[controllerIndex];
            if (card != null && controller != null) card.Controller = controller;
            else Debug.Log($"Could not set card controller, card: {card}; controller: {controller}");
        }
    }
}