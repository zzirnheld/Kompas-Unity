using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Effects
{
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

        public ServerPlayer ServerPlayer => ServerGame.ServerPlayers[(Controller.index + playerIndex) % ServerGame.ServerPlayers.Length];

        private static ServerSubeffect FromJson(string type, string subeffJson)
        {
            switch (type)
            {
                //targeting
                case BoardTarget: return JsonUtility.FromJson<BoardTargetSubeffect>(subeffJson);
                case DeckTarget: return JsonUtility.FromJson<DeckTargetSubeffect>(subeffJson);
                case DiscardTarget: return JsonUtility.FromJson<DiscardTargetSubeffect>(subeffJson);
                case HandTarget: return JsonUtility.FromJson<HandTargetSubeffect>(subeffJson);
                case TargetThis: return JsonUtility.FromJson<TargetThisSubeffect>(subeffJson);
                case TargetThisSpace: return JsonUtility.FromJson<TargetThisSpaceSubeffect>(subeffJson);
                case TargetAugmentedCard: return JsonUtility.FromJson<TargetAugmentedCardSubeffect>(subeffJson);
                case TargetTriggeringCard: return JsonUtility.FromJson<TargetTriggeringCardSubeffect>(subeffJson);
                case TargetTriggeringCoords: return JsonUtility.FromJson<TargetTriggeringCoordsSubeffect>(subeffJson);
                case ChooseFromList: return JsonUtility.FromJson<ChooseFromListSubeffect>(subeffJson);
                case ChooseFromListSaveRest: return JsonUtility.FromJson<ChooseFromListSaveRestSubeffect>(subeffJson);
                case DeleteTargetFromList: return JsonUtility.FromJson<DeleteTargetSubeffect>(subeffJson);
                case TargetAll: return JsonUtility.FromJson<TargetAllSubeffect>(subeffJson);
                case AddRest: return JsonUtility.FromJson<AddRestSubeffect>(subeffJson);
                case TargetDefender: return JsonUtility.FromJson<TargetDefenderSubeffect>(subeffJson);
                case TargetAttacker: return JsonUtility.FromJson<TargetAttackerSubeffect>(subeffJson);
                case SpaceTarget: return JsonUtility.FromJson<SpaceTargetSubeffect>(subeffJson);

                //change stats
                    //numeric
                case ChangeNESW: return JsonUtility.FromJson<ChangeNESWSubeffect>(subeffJson);
                case ChangeAllNESW: return JsonUtility.FromJson<ChangeAllNESWSubeffect>(subeffJson);
                case SetNESW: return JsonUtility.FromJson<SetNESWSubeffect>(subeffJson);
                case SetAllNESW: return JsonUtility.FromJson<SetAllNESWSubeffect>(subeffJson);
                case XChangeNESW: return JsonUtility.FromJson<XChangeNESWSubeffect>(subeffJson);
                case SwapNESW: return JsonUtility.FromJson<SwapNESWSubeffect>(subeffJson);
                case SwapOwnNESW: return JsonUtility.FromJson<SwapOwnNESWSubeffect>(subeffJson);
                case ChangeSpellC: return JsonUtility.FromJson<ChangeSpellCSubeffect>(subeffJson);
                case ResetStats: return JsonUtility.FromJson<ResetStatsSubeffect>(subeffJson);
                    //other
                case Negate: return JsonUtility.FromJson<NegateSubeffect>(subeffJson);
                case Activate: return JsonUtility.FromJson<ActivateSubeffect>(subeffJson);
                case Dispel: return JsonUtility.FromJson<DispelSubeffect>(subeffJson);
                case SpendMovement: return JsonUtility.FromJson<SpendMovementSubeffect>(subeffJson);
                case TakeControl: return JsonUtility.FromJson<TakeControlSubeffect>(subeffJson);
                case Resummon: return JsonUtility.FromJson<ResummonSubeffect>(subeffJson);
                case ResummonAll: return JsonUtility.FromJson<ResummonAllSubeffect>(subeffJson);

                //set/change x
                case SetXByBoardCount: return JsonUtility.FromJson<SetXBoardRestrictionSubeffect>(subeffJson);
                case SetXByGamestateValue: return JsonUtility.FromJson<SetXByGamestateSubeffect>(subeffJson);
                case SetXByMath: return JsonUtility.FromJson<SetXSubeffect>(subeffJson);
                case SetXByTargetValue: return JsonUtility.FromJson<SetXByTargetValueSubeffect>(subeffJson);
                case SetXByTargetS: return JsonUtility.FromJson<SetXTargetSSubeffect>(subeffJson);
                case SetXByTargetCost: return JsonUtility.FromJson<SetXByTargetCostSubeffect>(subeffJson);
                case PlayerChooseX: return JsonUtility.FromJson<PlayerChooseXSubeffect>(subeffJson);
                case ChangeXByGamestateValue: return JsonUtility.FromJson<ChangeXByGamestateSubeffect>(subeffJson);
                case ChangeXByTargetValue: return JsonUtility.FromJson<ChangeXByTargetValueSubeffect>(subeffJson);

                //pips
                case AddPips: return JsonUtility.FromJson<AddPipsSubeffect>(subeffJson);
                case PayPips: return JsonUtility.FromJson<PayPipsSubeffect>(subeffJson);
                case PayPipsByTargetCost: return JsonUtility.FromJson<PayPipsTargetCostSubeffect>(subeffJson);

                //move cards around
                case PlayCard: return JsonUtility.FromJson<PlaySubeffect>(subeffJson);
                case DiscardCard: return JsonUtility.FromJson<DiscardSubeffect>(subeffJson);
                case ReshuffleCard: return JsonUtility.FromJson<ReshuffleSubeffect>(subeffJson);
                case RehandCard: return JsonUtility.FromJson<RehandSubeffect>(subeffJson);
                case Draw: return JsonUtility.FromJson<DrawSubeffect>(subeffJson);
                case DrawX: return JsonUtility.FromJson<DrawXSubeffect>(subeffJson);
                case Bottomdeck: return JsonUtility.FromJson<BottomdeckSubeffect>(subeffJson);
                case Topdeck: return JsonUtility.FromJson<TopdeckSubeffect>(subeffJson);
                case Move: return JsonUtility.FromJson<MoveSubeffect>(subeffJson);
                case Swap: return JsonUtility.FromJson<SwapSubeffect>(subeffJson);
                case Annihilate: return JsonUtility.FromJson<AnnihilateSubeffect>(subeffJson);
                case Mill: return JsonUtility.FromJson<MillSubeffect>(subeffJson);
                case BottomdeckRest: return JsonUtility.FromJson<BottomdeckRestSubeffect>(subeffJson);

                //control flows
                case XTimesLoop: return JsonUtility.FromJson<XTimesSubeffect>(subeffJson);
                case TTimesLoop: return JsonUtility.FromJson<TTimesSubeffect>(subeffJson);
                case WhileHaveTargetsLoop: return JsonUtility.FromJson<LoopWhileHaveTargetsSubeffect>(subeffJson);
                case JumpOnImpossible: return JsonUtility.FromJson<SkipToEffectOnImpossibleSubeffect>(subeffJson);
                case ClearOnImpossible: return JsonUtility.FromJson<ClearOnImpossibleSubeffect>(subeffJson);
                case ChooseEffectOption: return JsonUtility.FromJson<ChooseOptionSubeffect>(subeffJson);
                case EndEffect: return JsonUtility.FromJson<EndResolutionSubeffect>(subeffJson);
                case CountXLoop: return JsonUtility.FromJson<CountXLoopSubeffect>(subeffJson);
                case ConditionalEndEffect: return JsonUtility.FromJson<ConditionalEndSubeffect>(subeffJson);
                case BasicLoop: return JsonUtility.FromJson<LoopSubeffect>(subeffJson); 
                case Jump: return JsonUtility.FromJson<JumpSubeffect>(subeffJson);
                case ConditionalJump: return JsonUtility.FromJson<ConditionalJumpSubeffect>(subeffJson);

                //hanging/delayed
                case HangingNESWBuff: return JsonUtility.FromJson<TemporaryNESWBuffSubeffect>(subeffJson); 
                case DelaySubeffect: return JsonUtility.FromJson<DelaySubeffect>(subeffJson);
                case HangingNESWBuffAll: return JsonUtility.FromJson<TemporaryNESWBuffAllSubeffect>(subeffJson);
                case HangingNegate:return JsonUtility.FromJson<TemporaryNegateSubeffect>(subeffJson);
                case HangingActivate: return JsonUtility.FromJson<TemporaryActivationSubeffect>(subeffJson);
                case HangingAnnihilate: return JsonUtility.FromJson<HangingAnnihilationSubeffect>(subeffJson);

                //misc
                case EndTurn: return JsonUtility.FromJson<EndTurnSubeffect>(subeffJson);
                case Attack: return JsonUtility.FromJson<AttackSubeffect>(subeffJson);
                case ChangeLeyload: return JsonUtility.FromJson<ChangeLeyloadSubeffect>(subeffJson);
                default: throw new System.ArgumentException($"Unrecognized effect type enum {type} for loading effect in effect constructor for json {subeffJson}");
            }
        }

        public static ServerSubeffect FromJson(string subeffJson, ServerEffect parent, int subeffIndex)
        {
            var subeff = JsonUtility.FromJson<Subeffect>(subeffJson);

            Debug.Log($"Creating subeffect from json of type {subeff.subeffType} json {subeffJson}");

            ServerSubeffect toReturn = FromJson(subeff.subeffType, subeffJson);
            toReturn?.Initialize(parent, subeffIndex);
            return toReturn;
        }

        /// <summary>
        /// Server Subeffect resolve method. Does whatever this type of subeffect does,
        /// then executes the next subeffect.<br></br>
        /// Usually, this involves a call to ResolveNextSubeffect(),
        /// but if the subeffect does control flow, it calles ResolveSubeffect for a specific thing.<br></br>
        /// TODO refactor to return an enum/int to represent different control flow states (negatives being special values):<br></br>
        ///     effect impossible, 
        ///     resolve next, 
        ///     resolve specific, 
        ///     awaiting player input.<br></br>
        /// <returns><see langword="true"/> if the effect finished resolving successfully, <see langword="false"/> if it's awaiting response</returns>
        /// </summary>
        public abstract bool Resolve();

        /// <summary>
        /// Sets up the subeffect with whatever necessary values.
        /// Usually also initializes any restrictions the effects are using.
        /// </summary>
        /// <param name="eff">The effect this subeffect is part of.</param>
        /// <param name="subeffIndex">The index in the subeffect array of its parent <paramref name="eff"/> this subeffect is.</param>
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
        public virtual bool OnImpossible()
        {
            Debug.Log($"Base On Impossible called for {GetType()}");
            ServerEffect.OnImpossible = null;
            return ServerEffect.EffectImpossible();
        }
    }
}