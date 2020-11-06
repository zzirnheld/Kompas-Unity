using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ServerHandSizeStackable : IServerStackable
    {
        private ServerPlayer serverController;
        public ServerPlayer ServerController => serverController;
        public Player Controller => ServerController;

        public GameCard Source => null;

        public ServerHandSizeStackable(ServerPlayer controller)
        {
            this.serverController = controller;
        }

        public void StartResolution(ActivationContext context)
        {
            //TODO: have it send the get hand sizes choices packet.
            //make sure to send with a list restriction whose min and max are
            //the number of cards that must be discarded
            //then make the client, if it is done picking targets,
            //and the target mode is handsize,
            //send a sendhandsizechocies packet that comes back to this.
        }
    }
}