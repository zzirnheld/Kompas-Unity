using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions
{
    public interface ICardRestrictionElement
    {
        public bool FitsRestriction(GameCardBase card);
    }

    public class CardExistsRestrictionElement : ICardRestrictionElement
    {
        public bool FitsRestriction(GameCardBase card) => card != null;
    }
}