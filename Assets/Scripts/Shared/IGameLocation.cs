using KompasCore.Cards;

public interface IGameLocation
{
    public CardLocation CardLocation { get; }

    public int IndexOf(GameCard card);

    public void Remove(GameCard card);
}
