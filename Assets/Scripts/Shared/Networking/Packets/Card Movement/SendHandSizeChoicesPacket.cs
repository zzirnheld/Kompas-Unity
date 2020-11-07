using KompasCore.Networking;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Networking
{
    public class SendHandSizeChoicesPacket : Packet
    {
        public int[] cardIds;

        public SendHandSizeChoicesPacket() : base(HandSizeChoices) { }

        public SendHandSizeChoicesPacket(int[] cardIds) : this()
        {
            Debug.Log($"Hand size choices {string.Join(", ", cardIds)}");
            this.cardIds = cardIds;
        }

        public override Packet Copy() => new SendHandSizeChoicesPacket(cardIds);
    }
}

namespace KompasServer.Networking
{
    public class SendHandSizeChoicesServerPacket : SendHandSizeChoicesPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            if(serverGame.EffectsController.CurrStackEntry is ServerHandSizeStackable stackable)
            {
                stackable.TryAnswer(cardIds);
            }
        }
    }
}