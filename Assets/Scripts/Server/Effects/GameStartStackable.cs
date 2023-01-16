using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class GameStartStackable : IStackable, IServerStackable
    {
        public Player Controller => null;

        public GameCard Source => null;

        public ServerPlayer ServerController => null;

        public GameCard GetCause(GameCardBase withRespectTo) => Source;

        public Task StartResolution(ActivationContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}