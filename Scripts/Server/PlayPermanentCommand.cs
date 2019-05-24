using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPermanentCommand : StackableCommand
{
    Card card;
    int x;
    int y;

    public PlayPermanentCommand(ServerGame game, Card card, int x, int y)
    {
        serverGame = game;
        this.card = card;
        this.x = x;
        this.y = y;
    }

    public override void StartResolution()
    {
        serverGame.Play(card, x, y);

        FinishResolution();
    }

    public override void CancelResolution()
    {
        card.Discard();
        FinishResolution();
    }
}
