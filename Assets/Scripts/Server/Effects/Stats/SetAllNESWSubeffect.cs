using System.Linq;

public class SetAllNESWSubeffect : SetNESWSubeffect
{
    private (int, int, int, int) GetRealValues(GameCard c)
    {
        (int n, int e, int s, int w) = (
            NVal >= 0 ? NVal : c.N,
            EVal >= 0 ? EVal : c.E,
            SVal >= 0 ? SVal : c.S,
            WVal >= 0 ? WVal : c.W
        );
        return (n, e, s, w);
    }

    //default to making sure things are characters before changing their stats
    public BoardRestriction BoardRestriction = new BoardRestriction()
    {
        restrictionsToCheck = new CardRestriction.CardRestrictions[]
        {
            CardRestriction.CardRestrictions.IsCharacter,
            CardRestriction.CardRestrictions.Board
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        BoardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        var targets = ServerGame.Cards.Where(c => BoardRestriction.Evaluate(c));
        foreach (ServerGameCard c in targets)
        {
            var (n, e, s, w) = GetRealValues(c);
            c.SetN(e, ServerEffect);
            c.SetE(e, ServerEffect);
            c.SetE(e, ServerEffect);
            c.SetE(e, ServerEffect);
        }
        ServerEffect.ResolveNextSubeffect();
    }
}
