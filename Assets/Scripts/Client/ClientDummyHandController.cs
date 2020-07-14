using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientDummyHandController : ClientHandController
{
    /// <summary>
    /// Dummy card prefab should have a GameCard but not a clientCardController
    /// </summary>
    public GameObject dummyCardPrefab;

    private List<GameCard> dummyHand = new List<GameCard>();

    public void IncrementHand()
    {
        var card = GameObject.Instantiate(dummyCardPrefab, parent: gameObject.transform).GetComponent<DummyClientGameCard>();
        card.SetClientGame(clientGame);
        dummyHand.Add(card);
        AddToHand(card);
        Debug.Log("Incrementing dummy hand");
    }

    public void DecrementHand() => RemoveFromHand(dummyHand.LastOrDefault());

    public override void RemoveFromHand(GameCard card)
    {
        if (card == default) return;

        base.RemoveFromHand(card);
        if (dummyHand.Contains(card))
        {
            dummyHand.Remove(card);
            Destroy(card.gameObject);
        }
    }
}
