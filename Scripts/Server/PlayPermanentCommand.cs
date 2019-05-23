using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPermanentCommand : StackableCommand
{
    Card card;
    int x;
    int y;

    public PlayPermanentCommand(Card card, int x, int y)
    {
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
