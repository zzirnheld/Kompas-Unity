using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerDeckController : DeckController
{
    public ServerGame ServerGame;

    public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
    public ServerEffectsController EffectsController => ServerGame.EffectsController;

    protected override void AddCard(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.ToDeck, context);
        base.AddCard(card);
    }

    public override void PushBottomdeck(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.Bottomdeck, context);
        ServerNotifier.NotifyBottomdeck(card);
        base.PushBottomdeck(card, stackSrc);
    }

    public override void PushTopdeck(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.Topdeck, context);
        ServerNotifier.NotifyTopdeck(card);
        base.PushTopdeck(card, stackSrc);
    }

    public override void ShuffleIn(GameCard card, IStackable stackSrc = null)
    {
        var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
        EffectsController.Trigger(TriggerCondition.Reshuffle, context);
        ServerNotifier.NotifyReshuffle(card);
        base.ShuffleIn(card, stackSrc);
    }
}
