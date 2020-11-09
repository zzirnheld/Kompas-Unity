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

    public void IncrementHand()
    {
        var card = GameObject.Instantiate(dummyCardPrefab, parent: gameObject.transform).GetComponent<DummyClientGameCard>();
        card.SetClientGame(clientGame);
        dummyHand.Add(card);
        AddToHand(card);
        Debug.Log("Incrementing dummy hand");
    }

    public void DecrementHand() => RemoveFromHand(dummyHand.LastOrDefault());

    public override bool RemoveFromHand(GameCard card)
    {
        //if it's a dummy, remove and destroy it
        if (dummyHand.Contains(card))
        {
            dummyHand.Remove(card);
            RemoveFromHand(card);
            Destroy(card.gameObject);
            return true;
        }

        //remove the card from the real hand if it's actually there
        return base.RemoveFromHand(card);
    }

    public override void SpreadOutCards()
    {
        base.SpreadOutCards();
        foreach(var c in hand) c.transform.eulerAngles = new Vector3(0, 0, 180);
    }
}
