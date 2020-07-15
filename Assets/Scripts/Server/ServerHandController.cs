using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandController : HandController
{
    public ServerGame ServerGame;

    public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
    public ServerEffectsController EffectsController => ServerGame.EffectsController;

    public override bool AddToHand(GameCard card, IStackable stackSrc = null)
    {
        if (!card.CanRemove) return false;
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.Rehand, context);
        ServerNotifier.NotifyRehand(card);
        return base.AddToHand(card);
    }
}
