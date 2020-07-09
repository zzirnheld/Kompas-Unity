using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerDiscardController : DiscardController
{
    public ServerGame ServerGame;

    public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
    public ServerEffectsController EffectsController => ServerGame.EffectsController;

    public override void AddToDiscard(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.Discard, context);
        ServerNotifier.NotifyDiscard(card);
        base.AddToDiscard(card);
    }
}
