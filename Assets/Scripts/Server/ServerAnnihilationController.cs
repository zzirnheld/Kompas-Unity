using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerAnnihilationController : AnnihilationController
{
    public ServerGame ServerGame;

    public override void Annihilate(GameCard card, IStackable stackSrc = null)
    {
        ServerGame.EffectsController.Trigger(TriggerCondition.Annhilate,
            cardTriggerer: card, stackTrigger: stackSrc);
        ServerGame.ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyAnnhilate(card);
        base.Annihilate(card, stackSrc);
    }
}
