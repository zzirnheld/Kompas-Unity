using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.GameCore;
using UnityEngine;


namespace KompasServer.Effects
{
    [System.Serializable]
    public class ServerEffect : Effect, IServerStackable
    {
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
            TimesUsedThisRound++;
            TimesUsedThisTurn++;
            TimesUsedThisStack++;
            serverGame = game;
            Controller = ctrl;
            ctrl.ServerNotifier.NotifyEffectActivated(this);
        }

        #region resolution
        public override void StartResolution(ActivationContext context)
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
            if (Negated) EffectImpossible();
            else ResolveSubeffect(context.StartIndex);
        }

        public bool ResolveNextSubeffect() => ResolveSubeffect(SubeffectIndex + 1);

        public bool ResolveSubeffect(int index)
        {
            if (index >= subeffects.Length)
            {
                FinishResolution();
                return true;
            }

            // Debug.Log($"Resolving subeffect of type {ServerSubeffects[index].GetType()}");
            SubeffectIndex = index;
            ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
            return subeffects[index].Resolve();
        }

        public bool EndResolution()
        {
            FinishResolution();
            return true;
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
            EffectsController.FinishStackEntryResolution();
        }

        /// <summary>
        /// Cancels resolution of the effect, 
        /// or, if there is something pending if the effect becomes impossible, resolves that
        /// </summary>
        public bool EffectImpossible()
        {
            Debug.Log($"Effect of {Source.CardName} is being declared impossible at subeffect {subeffects[SubeffectIndex].GetType()}");
            if (OnImpossible == null)
            {
                FinishResolution();
                ServerController.ServerNotifier.EffectImpossible();
                return false;
            }
            else
            {
                SubeffectIndex = OnImpossible.SubeffIndex;
                return OnImpossible.OnImpossible();
            }
        }
        #endregion resolution

        public void DeclineAnotherTarget() => OnImpossible?.OnImpossible();

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

            sb.Append($"Effect of {Source.CardName}:\n");
            foreach (var s in Subeffects) sb.Append($"{s.GetType()},\n");

            return sb.ToString();
        }
    }
}