using KompasClient.GameCore;
using KompasCore.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Networking
{
    public class HandSizeToStackPacket : Packet
    {
        public bool friendly;
        public string FriendlyString => friendly ? "Friendly" : "Enemy"; 

        public HandSizeToStackPacket() : base(HandSizeToStack) { }

        public HandSizeToStackPacket(bool friendly) : this()
        {
            this.friendly = friendly;
        }

        public override Packet Copy() => new HandSizeToStackPacket(friendly);

        public override Packet GetInversion(bool known = true) => new HandSizeToStackPacket(!friendly);
    }
}

namespace KompasClient.Networking
{
    public class HandSizeToStackClientPacket : HandSizeToStackPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.clientUICtrl.clientStackUICtrl.Add(null, FriendlyString, "Hand Size");
        }
    }
}