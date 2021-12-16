﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;

namespace KompasServer.GameCore
{
    public class ServerAnnihilationController : AnnihilationController
    {
        public ServerGame ServerGame;

        public override void Annihilate(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(beforeCard: card, stackable: stackSrc, triggerer: stackSrc?.Controller);
            bool wasKnown = card.KnownToEnemy;
            base.Annihilate(card, stackSrc);
            context.SetAfterCardInfo(card);
            ServerGame.EffectsController.TriggerForCondition(Trigger.Annhilate, context);
            ServerGame.ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyAnnhilate(card, wasKnown);
        }
    }
}