using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAnnihilationController : AnnihilationController
{
    public ServerGame ServerGame;

    public override void Annihilate(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: stackSrc?.Controller);
        ServerGame.EffectsController.Trigger(TriggerCondition.Annhilate, context);
        ServerGame.ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyAnnhilate(card);
        base.Annihilate(card, stackSrc);
    }
}
