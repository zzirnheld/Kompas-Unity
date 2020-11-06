using KompasCore.Networking;
using KompasServer.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Networking
{
    public class SendHandSizeChoicesPacket : Packet
    {
        int[] cardIds;

        public SendHandSizeChoicesPacket() : base(HandSizeChoices) { }

        public SendHandSizeChoicesPacket(int[] cardIds) : this()
        {
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
            throw new System.NotImplementedException();
        }
    }
}