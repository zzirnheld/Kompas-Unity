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
        var card = GameObject.Instantiate(dummyCardPrefab, parent: gameObject.transform).GetComponent<DummyClientGameCard>();
        card.SetClientGame(clientGame);
        dummyHand.Add(card);
        Add(card);
    }

    public override void DecrementHand() => Remove(dummyHand.LastOrDefault());

    public override void Remove(GameCard card)
    {
        //if it's a dummy, remove and destroy it
        if (dummyHand.Contains(card))
        {
            dummyHand.Remove(card);
            Remove(card);
            Destroy(card.gameObject);
            return;
        }

        //remove the card from the real hand if it's actually there
        base.Remove(card);
    }

    public override void SpreadOutCards()
    {
        base.SpreadOutCards();
        foreach (var c in hand) c.transform.eulerAngles = new Vector3(0, 0, 180);
    }
}
