using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerDeckController : DeckController
{
    public ServerGame ServerGame;

    public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
    public ServerEffectsController EffectsController => ServerGame.EffectsController;

    protected override bool AddCard(GameCard card, IStackable stackSrc = null)
    {
        if (card.CanRemove)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.Trigger(TriggerCondition.ToDeck, context);
            return base.AddCard(card);
        }
        return false;
    }

    public override bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
    {
        if (card.CanRemove)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.Trigger(TriggerCondition.Bottomdeck, context);
            ServerNotifier.NotifyBottomdeck(card);
            return base.PushBottomdeck(card, stackSrc);
        }
        return false;
    }

    public override bool PushTopdeck(GameCard card, IStackable stackSrc = null)
    {
        if (card.CanRemove)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.Trigger(TriggerCondition.Topdeck, context);
            ServerNotifier.NotifyTopdeck(card);
            return base.PushTopdeck(card, stackSrc);
        }
        return false;
    }

    public override bool ShuffleIn(GameCard card, IStackable stackSrc = null)
    {
        if (card.CanRemove)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.Trigger(TriggerCondition.Reshuffle, context);
            ServerNotifier.NotifyReshuffle(card);
            return base.ShuffleIn(card, stackSrc);
        }
        return false;
    }
}
