using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomdeckRestSubeffect : ServerSubeffect
{
    private static readonly System.Random rng = new System.Random();

    private List<Card> Shuffle(List<Card> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public override void Resolve()
    {
        //TODO better shuffling algorithm
        var list = Shuffle(Effect.Rest);
        foreach (Card c in list) ServerGame.Bottomdeck(c, ServerEffect);

        ServerEffect.ResolveNextSubeffect();
    }
}
