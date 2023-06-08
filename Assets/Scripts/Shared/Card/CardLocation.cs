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

	/// <summary>
	/// Should reflect the CardLocationHelpers.FromString function
	/// </summary>
	public static string StringVersion(this CardLocation cardLocation) => cardLocation switch
	{
		CardLocation.Nowhere => "Nowhere",
		CardLocation.Board => "Board",
		CardLocation.Hand => "Hand",
		CardLocation.Discard => "Discard",
		CardLocation.Annihilation => "Annihilation",
		CardLocation.Deck => "Deck",

		_ => throw new System.NotImplementedException($"Unknown CardLocation {cardLocation} to convert to string")
	};
}