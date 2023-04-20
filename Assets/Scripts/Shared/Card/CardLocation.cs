public enum CardLocation
{
    Nowhere, Board, Discard, Hand, Deck, Annihilation
}

public static class CardLocationHelpers
{
    public static CardLocation FromString(string str)
    {
        switch (str)
        {
            case "Board":
            case "Field":
                return CardLocation.Board;
            case "Hand":
                return CardLocation.Hand;
            case "Discard":
                return CardLocation.Discard;
            case "Annihilation":
                return CardLocation.Annihilation;
            case "Deck":
                return CardLocation.Deck;
            default:
                throw new System.NotImplementedException($"Unknown string to convert to CardLocation {str}");
        }
    }
}