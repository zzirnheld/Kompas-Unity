using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ServerSubeffect : Subeffect
{
    public override Player Controller => EffectController;
    public override Effect Effect => ServerEffect;
    public override Game Game => ServerGame;

    public ServerEffect ServerEffect { get; protected set; }
    public ServerGame ServerGame { get { return ServerEffect.serverGame; } }
    public ServerPlayer EffectController { get { return ServerEffect.ServerController; } }
    public GameCard ThisCard { get { return ServerEffect.Source; } }

    public static ServerSubeffect FromJson(string subeffJson, ServerEffect parent, int subeffIndex)
    {
        var subeff = JsonUtility.FromJson<Subeffect>(subeffJson);

        Debug.Log("Creating subeffect from json of type " + subeff.subeffType + " json " + subeffJson);

        ServerSubeffect toReturn;

        switch (subeff.subeffType)
        {
            case BoardTarget:
                toReturn = JsonUtility.FromJson<BoardTargetSubeffect>(subeffJson);
                break;
            case DeckTarget:
                toReturn = JsonUtility.FromJson<DeckTargetSubeffect>(subeffJson);
                break;
            case DiscardTarget:
                toReturn = JsonUtility.FromJson<DiscardTargetSubeffect>(subeffJson);
                break;
            case HandTarget:
                toReturn = JsonUtility.FromJson<HandTargetSubeffect>(subeffJson);
                break;
            case TargetThis:
                toReturn = JsonUtility.FromJson<TargetThisSubeffect>(subeffJson);
                break;
            case TargetThisSpace:
                toReturn = JsonUtility.FromJson<TargetThisSpaceSubeffect>(subeffJson);
                break;
            case TargetAugmentedCard:
                toReturn = JsonUtility.FromJson<TargetAugmentedCardSubeffect>(subeffJson);
                break;
            case ChooseFromList:
                toReturn = JsonUtility.FromJson<ChooseFromListSubeffect>(subeffJson);
                break;
            case ChooseFromListSaveRest:
                toReturn = JsonUtility.FromJson<ChooseFromListSaveRestSubeffect>(subeffJson);
                break;
            case DeleteTargetFromList:
                toReturn = JsonUtility.FromJson<DeleteTargetSubeffect>(subeffJson);
                break;
            case TargetAll:
                toReturn = JsonUtility.FromJson<TargetAllSubeffect>(subeffJson);
                break;
            case AddRest:
                toReturn = JsonUtility.FromJson<AddRestSubeffect>(subeffJson);
                break;
            case TargetDefender:
                toReturn = JsonUtility.FromJson<TargetDefenderSubeffect>(subeffJson);
                break;
            case ChangeNESW:
                toReturn = JsonUtility.FromJson<ChangeNESWSubeffect>(subeffJson);
                break;
            case XChangeNESW:
                toReturn = JsonUtility.FromJson<XChangeNESWSubeffect>(subeffJson);
                break;
            case SwapNESW:
                toReturn = JsonUtility.FromJson<SwapNESWSubeffect>(subeffJson);
                break;
            case AddPips:
                toReturn = JsonUtility.FromJson<AddPipsSubeffect>(subeffJson);
                break;
            case Negate:
                toReturn = JsonUtility.FromJson<NegateSubeffect>(subeffJson);
                break;
            case Activate:
                toReturn = JsonUtility.FromJson<ActivateSubeffect>(subeffJson);
                break;
            case Dispel:
                toReturn = JsonUtility.FromJson<DispelSubeffect>(subeffJson);
                break;
            case SwapOwnNESW:
                toReturn = JsonUtility.FromJson<SwapOwnNESWSubeffect>(subeffJson);
                break;
            case ChangeSpellC:
                toReturn = JsonUtility.FromJson<ChangeSpellCSubeffect>(subeffJson);
                break;
            case SetNESW:
                toReturn = JsonUtility.FromJson<SetNESWSubeffect>(subeffJson);
                break;
            case ChangeAllNESW:
                toReturn = JsonUtility.FromJson<ChangeAllNESWSubeffect>(subeffJson);
                break;
            case SetAllNESW:
                toReturn = JsonUtility.FromJson<SetAllNESWSubeffect>(subeffJson);
                break;
            case SpendMovement:
                toReturn = JsonUtility.FromJson<SpendMovementSubeffect>(subeffJson);
                break;
            case TakeControl:
                toReturn = JsonUtility.FromJson<TakeControlSubeffect>(subeffJson);
                break;
            case ResetStats:
                toReturn = JsonUtility.FromJson<ResetStatsSubeffect>(subeffJson);
                break;
            case SetXByBoardCount:
                toReturn = JsonUtility.FromJson<SetXBoardRestrictionSubeffect>(subeffJson);
                break;
            case SetXByGamestateValue:
                toReturn = JsonUtility.FromJson<SetXByGamestateSubeffect>(subeffJson);
                break;
            case SetXByMath:
                toReturn = JsonUtility.FromJson<SetXSubeffect>(subeffJson);
                break;
            case SetXByTargetValue:
                toReturn = JsonUtility.FromJson<SetXByTargetValueSubeffect>(subeffJson);
                break;
            case ChangeXByGamestateValue:
                toReturn = JsonUtility.FromJson<ChangeXByGamestateSubeffect>(subeffJson);
                break;
            case ChangeXByTargetValue:
                toReturn = JsonUtility.FromJson<ChangeXByTargetValueSubeffect>(subeffJson);
                break;
            case PlayerChooseX:
                toReturn = JsonUtility.FromJson<PlayerChooseXSubeffect>(subeffJson);
                break;
            case SpaceTarget:
                toReturn = JsonUtility.FromJson<SpaceTargetSubeffect>(subeffJson);
                break;
            case PayPips:
                toReturn = JsonUtility.FromJson<PayPipsSubeffect>(subeffJson);
                break;
            case SetXByTargetS:
                toReturn = JsonUtility.FromJson<SetXTargetSSubeffect>(subeffJson);
                break;
            case SetXByTargetCost:
                toReturn = JsonUtility.FromJson<SetXByTargetCostSubeffect>(subeffJson);
                break;
            case PlayCard:
                toReturn = JsonUtility.FromJson<PlaySubeffect>(subeffJson);
                break;
            case PayPipsByTargetCost:
                toReturn = JsonUtility.FromJson<PayPipsTargetCostSubeffect>(subeffJson);
                break;
            case DiscardCard:
                toReturn = JsonUtility.FromJson<DiscardSubeffect>(subeffJson);
                break;
            case ReshuffleCard:
                toReturn = JsonUtility.FromJson<ReshuffleSubeffect>(subeffJson);
                break;
            case RehandCard:
                toReturn = JsonUtility.FromJson<RehandSubeffect>(subeffJson);
                break;
            case Draw:
                toReturn = JsonUtility.FromJson<DrawSubeffect>(subeffJson);
                break;
            case DrawX:
                toReturn = JsonUtility.FromJson<DrawXSubeffect>(subeffJson);
                break;
            case Bottomdeck:
                toReturn = JsonUtility.FromJson<BottomdeckSubeffect>(subeffJson);
                break;
            case Topdeck:
                toReturn = JsonUtility.FromJson<TopdeckSubeffect>(subeffJson);
                break;
            case Move:
                toReturn = JsonUtility.FromJson<MoveSubeffect>(subeffJson);
                break;
            case Swap:
                toReturn = JsonUtility.FromJson<SwapSubeffect>(subeffJson);
                break;
            case Annihilate:
                toReturn = JsonUtility.FromJson<AnnihilateSubeffect>(subeffJson);
                break;
            case BottomdeckRest:
                toReturn = JsonUtility.FromJson<BottomdeckRestSubeffect>(subeffJson);
                break;
            case XTimesLoop:
                toReturn = JsonUtility.FromJson<XTimesSubeffect>(subeffJson);
                break;
            case TTimesLoop:
                toReturn = JsonUtility.FromJson<TTimesSubeffect>(subeffJson);
                break;
            case WhileHaveTargetsLoop:
                toReturn = JsonUtility.FromJson<LoopWhileHaveTargetsSubeffect>(subeffJson);
                break;
            case ExitLoopIfEffectImpossible:
                toReturn = JsonUtility.FromJson<ExitLoopIfEffectImpossibleSubeffect>(subeffJson);
                break;
            case JumpOnImpossible:
                toReturn = JsonUtility.FromJson<SkipToEffectOnImpossibleSubeffect>(subeffJson);
                break;
            case ClearOnImpossible:
                toReturn = JsonUtility.FromJson<ClearOnImpossibleSubeffect>(subeffJson);
                break;
            case ChooseEffectOption:
                toReturn = JsonUtility.FromJson<ChooseOptionSubeffect>(subeffJson);
                break;
            case EndEffect:
                toReturn = JsonUtility.FromJson<EndResolutionSubeffect>(subeffJson);
                break;
            case CountXLoop:
                toReturn = JsonUtility.FromJson<CountXLoopSubeffect>(subeffJson);
                break;
            case ConditionalEndEffect:
                toReturn = JsonUtility.FromJson<ConditionalEndSubeffect>(subeffJson);
                break;
            case BasicLoop:
                toReturn = JsonUtility.FromJson<LoopSubeffect>(subeffJson);
                break;
            case Jump:
                toReturn = JsonUtility.FromJson<JumpSubeffect>(subeffJson);
                break;
            case HangingNESWBuff:
                toReturn = JsonUtility.FromJson<TemporaryNESWBuffSubeffect>(subeffJson);
                break;
            case DelaySubeffect:
                toReturn = JsonUtility.FromJson<DelaySubeffect>(subeffJson);
                break;
            case HangingNESWBuffAll:
                toReturn = JsonUtility.FromJson<TemporaryNESWBuffAllSubeffect>(subeffJson);
                break;
            case HangingNegate:
                toReturn = JsonUtility.FromJson<TemporaryNegateSubeffect>(subeffJson);
                break;
            case HangingActivate:
                toReturn = JsonUtility.FromJson<TemporaryActivationSubeffect>(subeffJson);
                break;
            case HangingAnnihilate:
                toReturn = JsonUtility.FromJson<HangingAnnihilationSubeffect>(subeffJson);
                break;
            case EndTurn:
                toReturn = JsonUtility.FromJson<EndTurnSubeffect>(subeffJson);
                break;
            case Attack:
                toReturn = JsonUtility.FromJson<AttackSubeffect>(subeffJson);
                break;
            default:
                Debug.LogError($"Unrecognized effect type enum {subeff.subeffType} for loading effect in effect constructor");
                return null;
        }

        if (toReturn != null) toReturn.Initialize(parent, subeffIndex);
        return toReturn;
    }

    /// <summary>
    /// parent resolve method. at the end, needs to call resolve subeffect in parent
    /// if it's an if, it does a specific index
    /// otherwise, it does currentIndex + 1
    /// </summary>
    public abstract void Resolve();

    public virtual void Initialize(ServerEffect eff, int subeffIndex)
    {
        Debug.Log($"Finishing setup for new effect of type {GetType()}");
        this.ServerEffect = eff;
        this.SubeffIndex = subeffIndex;
    }

    /// <summary>
    /// Optional method. If implemented, does something when the effect is declared impossible.
    /// Default implementation just finishes resolution of the effect
    /// </summary>
    public virtual void OnImpossible() {
        Debug.Log($"On Impossible called for {GetType()} without an override");
        ServerEffect.ResolveSubeffect(ServerEffect.ServerSubeffects.Length);
    }
}