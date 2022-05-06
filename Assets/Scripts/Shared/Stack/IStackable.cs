﻿using KompasCore.Cards;

namespace KompasCore.Effects
{
    public interface IStackable
    {
        Player Controller { get; }
        GameCard Source { get; }

        GameCard GetCause (GameCard withRespectTo);
    }
}