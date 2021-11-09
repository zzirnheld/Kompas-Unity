﻿using KompasCore.Cards;

namespace KompasCore.Effects
{
    public abstract class HandSizeStackable : IStackable
    {
        public const int MaxHandSize = 7;

        public abstract Player Controller { get; }

        public GameCard Source => Controller.Avatar;

        private ListRestriction handSizeListRestriction;
        public ListRestriction HandSizeListRestriction
        {
            get
            {
                if (handSizeListRestriction == null)
                {
                    handSizeListRestriction = new ListRestriction()
                    {
                        listRestrictions = new string[]
                        {
                            ListRestriction.MaxCanChoose, ListRestriction.MinCanChoose
                        }
                    };
                }
                return handSizeListRestriction;
            }
        }

        private CardRestriction handSizeCardRestriction;
        public CardRestriction HandSizeCardRestriction 
        {
            get {
                if (handSizeCardRestriction == null)
                {
                    handSizeCardRestriction = new CardRestriction()
                    {
                        cardRestrictions = new string[]
                        {
                            CardRestriction.Friendly, CardRestriction.Hand
                        }
                    };
                    handSizeCardRestriction.Initialize(Source, eff: default);
                }
                return handSizeCardRestriction;
            }
        }
    }
}