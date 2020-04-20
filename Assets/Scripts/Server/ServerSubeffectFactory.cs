using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSubeffectFactory : ISubeffectFactory
{
    public Subeffect FromJson(SubeffectType seType, string subeffJson, Effect parent, int subeffIndex)
    {
        Debug.Log("Creating subeffect from json of type " + seType + " json " + subeffJson);

        Subeffect toReturn = null;

        switch (seType)
        {
            case SubeffectType.BoardTarget:
                toReturn = JsonUtility.FromJson<BoardTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.DeckTarget:
                toReturn = JsonUtility.FromJson<DeckTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.DiscardTarget:
                toReturn = JsonUtility.FromJson<DiscardTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.HandTarget:
                toReturn = JsonUtility.FromJson<HandTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetThis:
                toReturn = JsonUtility.FromJson<TargetThisSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetThisSpace:
                toReturn = JsonUtility.FromJson<TargetThisSpaceSubeffect>(subeffJson);
                break;
            case SubeffectType.ChooseFromList:
                toReturn = JsonUtility.FromJson<ChooseFromListSubeffect>(subeffJson);
                break;
            case SubeffectType.ChooseFromListSaveRest:
                toReturn = JsonUtility.FromJson<ChooseFromListSaveRestSubeffect>(subeffJson);
                break;
            case SubeffectType.DeleteTargetFromList:
                toReturn = JsonUtility.FromJson<DeleteTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetAll:
                toReturn = JsonUtility.FromJson<TargetAllSubeffect>(subeffJson);
                break;
            case SubeffectType.ChangeNESW:
                toReturn = JsonUtility.FromJson<ChangeNESWSubeffect>(subeffJson);
                break;
            case SubeffectType.XChangeNESW:
                toReturn = JsonUtility.FromJson<XChangeNESWSubeffect>(subeffJson);
                break;
            case SubeffectType.AddPips:
                toReturn = JsonUtility.FromJson<AddPipsSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByBoardCount:
                toReturn = JsonUtility.FromJson<SetXBoardRestrictionSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByGamestateValue:
                toReturn = JsonUtility.FromJson<SetXByGamestateSubeffect>(subeffJson);
                break;
            case SubeffectType.ChangeXByGamestateValue:
                toReturn = JsonUtility.FromJson<ChangeXByGamestateSubeffect>(subeffJson);
                break;
            case SubeffectType.PlayerChooseX:
                toReturn = JsonUtility.FromJson<PlayerChooseXSubeffect>(subeffJson);
                break;
            case SubeffectType.SpaceTarget:
                toReturn = JsonUtility.FromJson<SpaceTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.PayPips:
                toReturn = JsonUtility.FromJson<PayPipsSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByTargetS:
                toReturn = JsonUtility.FromJson<SetXTargetSSubeffect>(subeffJson);
                break;
            case SubeffectType.PlayCard:
                toReturn = JsonUtility.FromJson<PlaySubeffect>(subeffJson);
                break;
            case SubeffectType.PayPipsByTargetCost:
                toReturn = JsonUtility.FromJson<PayPipsTargetCostSubeffect>(subeffJson);
                break;
            case SubeffectType.DiscardCard:
                toReturn = JsonUtility.FromJson<DiscardSubeffect>(subeffJson);
                break;
            case SubeffectType.ReshuffleCard:
                toReturn = JsonUtility.FromJson<ReshuffleSubeffect>(subeffJson);
                break;
            case SubeffectType.RehandCard:
                toReturn = JsonUtility.FromJson<RehandSubeffect>(subeffJson);
                break;
            case SubeffectType.Draw:
                toReturn = JsonUtility.FromJson<DrawSubeffect>(subeffJson);
                break;
            case SubeffectType.DrawX:
                toReturn = JsonUtility.FromJson<DrawXSubeffect>(subeffJson);
                break;
            case SubeffectType.Bottomdeck:
                toReturn = JsonUtility.FromJson<BottomdeckSubeffect>(subeffJson);
                break;
            case SubeffectType.Topdeck:
                toReturn = JsonUtility.FromJson<TopdeckSubeffect>(subeffJson);
                break;
            case SubeffectType.Move:
                toReturn = JsonUtility.FromJson<MoveSubeffect>(subeffJson);
                break;
            case SubeffectType.XTimesLoop:
                toReturn = JsonUtility.FromJson<XTimesSubeffect>(subeffJson);
                break;
            case SubeffectType.TTimesLoop:
                toReturn = JsonUtility.FromJson<TTimesSubeffect>(subeffJson);
                break;
            case SubeffectType.WhileHaveTargetsLoop:
                toReturn = JsonUtility.FromJson<LoopWhileHaveTargetsSubeffect>(subeffJson);
                break;
            case SubeffectType.ExitLoopIfEffectImpossible:
                toReturn = JsonUtility.FromJson<ExitLoopIfEffectImpossibleSubeffect>(subeffJson);
                break;
            case SubeffectType.JumpOnImpossible:
                toReturn = JsonUtility.FromJson<SkipToEffectOnImpossibleSubeffect>(subeffJson);
                break;
            case SubeffectType.ClearOnImpossible:
                toReturn = JsonUtility.FromJson<ClearOnImpossibleSubeffect>(subeffJson);
                break;
            case SubeffectType.ChooseEffectOption:
                toReturn = JsonUtility.FromJson<ChooseOptionSubeffect>(subeffJson);
                break;
            case SubeffectType.EndEffect:
                toReturn = JsonUtility.FromJson<EndResolutionSubeffect>(subeffJson);
                break;
            case SubeffectType.CountXLoop:
                toReturn = JsonUtility.FromJson<CountXLoopSubeffect>(subeffJson);
                break;
            case SubeffectType.ConditionalEndEffect:
                toReturn = JsonUtility.FromJson<ConditionalEndSubeffect>(subeffJson);
                break;
            default:
                Debug.LogError($"Unrecognized effect type enum {seType} for loading effect in effect constructor");
                return null;
        }

        if (toReturn != null)
        {
            Debug.Log($"Finishing setup for new effect of type {seType}");
            toReturn.Initialize(parent, subeffIndex);
        }

        return toReturn;
    }
}
