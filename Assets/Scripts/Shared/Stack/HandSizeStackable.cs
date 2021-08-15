using KompasCore.Cards;

namespace KompasCore.Effects
{
    public abstract class HandSizeStackable : IStackable
    {
        public const int MaxHandSize = 7;

        public abstract Player Controller { get; }

        public GameCard Source => Controller.Avatar;

        public ListRestriction HandSizeListRestriction
        {
            get
            {
                var l = new ListRestriction()
                {
                    listRestrictions = new string[]
                    {
                        ListRestriction.MaxCanChoose, ListRestriction.MinCanChoose
                    }
                };
                return l;
            }
        }

        public CardRestriction HandSizeCardRestriction 
        {
            get {
                var c = new CardRestriction()
                {
                    cardRestrictions = new string[]
                    {
                    CardRestriction.Friendly, CardRestriction.Hand
                    }
                };
                c.Initialize(Source, eff: default);
                return c;
            }
        }
    }
}