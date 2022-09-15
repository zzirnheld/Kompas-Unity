using KompasClient.Cards;
using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientDummyHandController : ClientHandController
{
    /// <summary>
    /// Dummy card prefab should have a GameCard but not a clientCardController
    /// </summary>
    public GameObject dummyCardPrefab;

    private readonly List<GameCard> dummyHand = new List<GameCard>();

    public override void IncrementHand()
    {
        var ctrl = Instantiate(dummyCardPrefab, parent: gameObject.transform).GetComponent<ClientCardController>();
        var card = new DummyClientGameCard(owner, ctrl);
        dummyHand.Add(card);
        Hand(card);
    }

    public override void DecrementHand() => Remove(dummyHand.LastOrDefault());

    public override void Remove(GameCard card)
    {
        //if it's a dummy, remove and destroy it
        if (dummyHand.Contains(card))
        {
            dummyHand.Remove(card);
            Remove(card);
            Destroy(card.CardController.gameObject);
            return;
        }

        //remove the card from the real hand if it's actually there
        base.Remove(card);
    }
}
