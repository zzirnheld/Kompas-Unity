using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameLocation
{
    public CardLocation CardLocation { get; }

    public void Remove(GameCard card);
}
