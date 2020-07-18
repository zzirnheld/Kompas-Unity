using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionalEndSubeffect : ServerSubeffect
{
    public const string XLessThan0 = "X<0";
    public const string XLessThanEqual0 = "X<=0";
    public const string XGreaterThanConst = "X>C";
    public const string XLessThanConst = "X<C";
    public const string NoneFitRestriction = "None Fit Restriction";
    public const string MustBeFriendlyTurn = "Must be Friendly Turn";

    public int constant = 0;
    public CardRestriction cardRestriction = new CardRestriction();

    public string condition;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }

    public override bool Resolve()
    {
        bool end;
        switch (condition)
        {
            case XLessThan0:
                end = ServerEffect.X < 0;
                break;
            case XLessThanEqual0:
                end = ServerEffect.X <= 0;
                break;
            case XGreaterThanConst:
                end = ServerEffect.X > constant;
                break;
            case XLessThanConst:
                end = ServerEffect.X < constant;
                break;
            case NoneFitRestriction:
                end = !ServerGame.Cards.Any(c => cardRestriction.Evaluate(c));
                break;
            case MustBeFriendlyTurn:
                end = ServerGame.TurnPlayer != Effect.Controller;
                break;
            default:
                throw new System.ArgumentException($"Condition {condition} invalid for conditional end subeffect");
        }

        if (end) return ServerEffect.EndResolution();
        else return ServerEffect.ResolveNextSubeffect();
    }
}
