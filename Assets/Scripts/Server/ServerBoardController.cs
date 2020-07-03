using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBoardController : BoardController
{
    public ServerGame ServerGame;

    public ServerNotifier ServerNotifierByIndex(int index) => ServerGame.ServerPlayers[index].ServerNotifier;
    public ServerEffectsController EffectsController => ServerGame.EffectsController;

    public override void Play(GameCard toPlay, int toX, int toY, Player controller, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Play,
            cardTriggerer: toPlay, stackTrigger: stackSrc, triggerer: controller, space: (toX, toY));
        if (!toPlay.IsAvatar) ServerNotifierByIndex(toPlay.ControllerIndex).NotifyPlay(toPlay, toX, toY);
        base.Play(toPlay, toX, toY, controller);
    }

    public override void Swap(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
    {
        EffectsController.Trigger(TriggerCondition.Move,
            cardTriggerer: card, stackTrigger: stackSrc, triggerer: card.Controller, space: (toX, toY), x: card.DistanceTo(toX, toY));
        ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, toX, toY, playerInitiated);
        var at = GetCardAt(toX, toY);
        if (at != null)
        {
            EffectsController.Trigger(TriggerCondition.Move,
                cardTriggerer: at, stackTrigger: stackSrc, triggerer: at.Controller, space: (card.BoardX, card.BoardY));
        }
        base.Swap(card, toX, toY, playerInitiated);
    }
}
