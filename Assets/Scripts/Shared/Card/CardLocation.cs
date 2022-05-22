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
            default:
                throw new System.NotImplementedException($"Unknown string to convert to CardLocation {str}");
        }
    }
}