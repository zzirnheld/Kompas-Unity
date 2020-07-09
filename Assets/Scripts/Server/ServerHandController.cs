using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandController : HandController
{
    public ServerGame ServerGame;

    public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
    public ServerEffectsController EffectsController => ServerGame.EffectsController;

    public override void AddToHand(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.Rehand, context);
        ServerNotifier.NotifyRehand(card);
        base.AddToHand(card);
    }
}
