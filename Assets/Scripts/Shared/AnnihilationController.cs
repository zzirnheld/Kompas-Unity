using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilationController : MonoBehaviour
{
    public Game game;

    public List<GameCard> Cards { get; } = new List<GameCard>();

    public virtual bool Annihilate(GameCard card, IStackable stackSrc = null)
    {
        card.Remove();
        Cards.Add(card);
        card.Location = CardLocation.Annihilation;
        return true;
    }

    public void Remove(GameCard card) => Cards.Remove(card);
}
