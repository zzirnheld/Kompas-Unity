using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.GameCore;
using System.Threading.Tasks;
using UnityEngine;


namespace KompasServer.Effects
{
    [System.Serializable]
    public class ServerEffect : Effect, IServerStackable
    {
        public const string EffectWasNegated = "Effect was negated";

        public ServerGame serverGame;
        public override Game Game => serverGame;
        public ServerEffectsController EffectsController => serverGame.EffectsController;
        public ServerPlayer ServerController { get; set; }
        public override Player Controller
        {
            get => ServerController;
            set => ServerController = value as ServerPlayer;
        }

        public ServerSubeffect[] subeffects;
        public override Subeffect[] Subeffects => subeffects;
        public ServerTrigger ServerTrigger { get; private set; }
        public override Trigger Trigger => ServerTrigger;

        public ServerSubeffect OnImpossible = null;
        public bool CanDeclineTarget = false;

        public void SetInfo(GameCard thisCard, ServerGame serverGame, ServerPlayer controller, int effectIndex)
        {
            base.SetInfo(thisCard, effectIndex, controller);
            this.serverGame = serverGame;
            this.ServerController = controller;

            if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
                ServerTrigger = new ServerTrigger(triggerData, this);

            int i = 0;
            foreach (var subeff in subeffects) subeff.Initialize(this, i++);
        }

        public override bool CanBeActivatedBy(Player controller)
            => serverGame.uiCtrl.DebugMode || base.CanBeActivatedBy(controller);

        public void PushedToStack(ServerGame game, ServerPlayer ctrl)
        {
            ActivationContext context = new ActivationContext(stackable: this);
            EffectsController.TriggerForCondition(Trigger.EffectPushedToStack, context);
            TimesUsedThisRound++;
            TimesUsedThisTurn++;
            TimesUsedThisStack++;
            serverGame = game;
            Controller = ctrl;
            ctrl.ServerNotifier.NotifyEffectActivated(this);
        }

        public override void Negate()
        {
            base.Negate();
            EffectsController.Cancel(this);
        }

        #region resolution
        public async Task StartResolution(ActivationContext context)
        {
            Debug.Log($"Resolving effect {EffectIndex} of {Source.CardName} in context {context}");
            serverGame.CurrEffect = this;

            //set context parameters
            CurrActivationContext = context;
            X = context.X ?? 0;

            targetsList.Clear();
            if (context.Targets != null) targetsList.AddRange(context.Targets);

            coords.Clear();
            
            players.Clear();
            players.Add(Controller);

            //notify relevant to this effect starting
            ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
            ServerController.ServerNotifier.EffectResolving(this);

            //resolve the effect if possible
            if (Negated) await EffectImpossible(EffectWasNegated);
            else await Resolve(context.StartIndex);

            //after all subeffects have finished, clean up
            FinishResolution();

            //then return. server effects controller will interpret returning as effect being done.
        }

        private async Task Resolve(int index)
        {
            //get first result
            ResolutionInfo result = await ResolveSubeffect(index);

            //then, so long as we should keep going, resolve subeffects
            bool resolve = true;
            while (resolve)
            {
                switch (result.result)
                {
                    case ResolutionResult.Next:
                        index++;
                        if (index < subeffects.Length) result = await ResolveSubeffect(index);
                        else resolve = false; //stop if next subeffect index is out of bounds
                        break;
                    case ResolutionResult.Index:
                        index = result.index;
                        if (index < subeffects.Length) result = await ResolveSubeffect(index);
                        else resolve = false; //stop if that subeffect index is out of bounds
                        break;
                    case ResolutionResult.Impossible:
                        Debug.Log($"Effect of {Source.CardName} was impossible at index {index} because {result.reason}. Going to OnImpossible if applicable");
                        result = await EffectImpossible(result.reason);
                        break;
                    case ResolutionResult.End:
                        //TODO send to player why resolution ended (including "[cardname] effect finished resolving")
                        Debug.Log($"Finished resolution of effect of {Source.CardName} because {result.reason}");
                        resolve = false;
                        break;
                    default:
                        throw new System.ArgumentException($"Invalid resolution result {result.result}");
                }
            }
        }

        public async Task<ResolutionInfo> ResolveSubeffect(int index)
        {
            Debug.Log($"Resolving subeffect of type {subeffects[index].GetType()}");
            SubeffectIndex = index;
            ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
            return await subeffects[index].Resolve();
        }

        /// <summary>
        /// If the effect finishes resolving, this method is called.
        /// </summary>
        private void FinishResolution()
        {
            SubeffectIndex = 0;
            X = 0;
            targetsList.Clear();
            rest.Clear();
            OnImpossible = null;
            ServerController.ServerNotifier.NotifyBothPutBack();
        }

        /// <summary>
        /// Cancels resolution of the effect, 
        /// or, if there is something pending if the effect becomes impossible, resolves that
        /// </summary>
        public async Task<ResolutionInfo> EffectImpossible(string why)
        {
            Debug.Log($"Effect of {Source.CardName} is being declared impossible at subeffect {subeffects[SubeffectIndex].GetType()}");
            if (OnImpossible == null)
            {
                //TODO make the notifier tell the client why the effect was impossible
                ServerController.ServerNotifier.EffectImpossible();
                return ResolutionInfo.End(ResolutionInfo.EndedBecauseImpossible);
            }
            else
            {
                SubeffectIndex = OnImpossible.SubeffIndex;
                return await OnImpossible.OnImpossible(why);
            }
        }
        #endregion resolution

        public override void AddTarget(GameCard card)
        {
            base.AddTarget(card);
            serverGame.ServerControllerOf(card).ServerNotifier.SetTarget(Source, EffectIndex, card);
        }

        public override void RemoveTarget(GameCard card)
        {
            base.RemoveTarget(card);
            serverGame.ServerControllerOf(card).ServerNotifier.RemoveTarget(Source, EffectIndex, card);
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append($"Effect of {Source.CardName}: ");
            foreach (var s in Subeffects) sb.Append($"{s.GetType()}, ");

            return sb.ToString();
        }
    }
}