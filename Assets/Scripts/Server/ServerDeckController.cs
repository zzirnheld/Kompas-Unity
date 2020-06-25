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
        EffectsController.Trigger(TriggerCondition.ToDeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: Owner);
        base.AddCard(card);
    }

    public override void PushBottomdeck(GameCard card, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Bottomdeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: Owner);
        ServerNotifier.NotifyBottomdeck(card);
        base.PushBottomdeck(card, stackSrc);
    }

    public override void PushTopdeck(GameCard card, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Topdeck, cardTriggerer: card, stackTrigger: stackSrc, triggerer: Owner);
        ServerNotifier.NotifyTopdeck(card);
        base.PushTopdeck(card, stackSrc);
    }

    public override void ShuffleIn(GameCard card, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Reshuffle, cardTriggerer: card, stackTrigger: stackSrc, triggerer: Owner);
        ServerNotifier.NotifyReshuffle(card);
        base.ShuffleIn(card, stackSrc);
    }
}
