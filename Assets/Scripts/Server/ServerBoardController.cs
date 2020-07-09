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
        var context = new ActivationContext(card: toPlay, stackable: stackSrc, triggerer: controller, space: (toX, toY));
        EffectsController.Trigger(TriggerCondition.Play, context);
        if (!toPlay.IsAvatar) ServerNotifierByIndex(toPlay.ControllerIndex).NotifyPlay(toPlay, toX, toY);
        base.Play(toPlay, toX, toY, controller);
    }

    public override void Swap(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
    {
        var contextA = new ActivationContext(card: card, stackable: stackSrc, space: (toX, toY),
            triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: card.DistanceTo(toX, toY));
        EffectsController.Trigger(TriggerCondition.Move, contextA);
        ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, toX, toY, playerInitiated);
        var at = GetCardAt(toX, toY);
        if (at != null)
        {
            var contextB = new ActivationContext(card: at, stackable: stackSrc, space: (toX, toY),
                triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: at.DistanceTo(card.BoardX, card.BoardY));
            EffectsController.Trigger(TriggerCondition.Move, contextB);
        }
        base.Swap(card, toX, toY, playerInitiated);
    }
}
