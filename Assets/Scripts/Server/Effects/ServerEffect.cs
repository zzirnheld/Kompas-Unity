using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using UnityEngine;


namespace KompasServer.Effects
{
    public class ServerEffect : Effect, IServerStackable
    {
        public ServerSubeffect[] ServerSubeffects { get; }
        public ServerTrigger ServerTrigger { get; }

        public ServerGame serverGame;
        public ServerEffectsController EffectsController => serverGame.EffectsController;

        public ServerSubeffect OnImpossible = null;

        public ServerPlayer ServerController { get; set; }
        public override Player Controller
        {
            get { return ServerController; }
            set { ServerController = value as ServerPlayer; }
        }
        public override Subeffect[] Subeffects => ServerSubeffects;
        public override Trigger Trigger => ServerTrigger;

        public ServerEffect(SerializableEffect se, GameCard thisCard, ServerGame serverGame, ServerPlayer controller, int effectIndex)
            : base(se.activationRestriction ?? new ActivationRestriction(), thisCard, se.blurb, effectIndex)
        {
            this.serverGame = serverGame;
            this.ServerController = controller;
            ServerSubeffects = new ServerSubeffect[se.subeffects.Length];

            if (!string.IsNullOrEmpty(se.trigger))
            {
                try
                {
                    ServerTrigger = ServerTrigger.FromJson(se.triggerCondition, se.trigger, this);
                    EffectsController.RegisterTrigger(se.triggerCondition, ServerTrigger);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogError($"Failed to load trigger of type {se.triggerCondition} from json {se.trigger}");
                    throw;
                }
            }

            for (int i = 0; i < se.subeffects.Length; i++)
            {
                try
                {
                    ServerSubeffects[i] = ServerSubeffect.FromJson(se.subeffects[i], this, i);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogError($"Failed to load subeffect from json {se.subeffects[i]}");
                    throw;
                }
            }
        }

        public override bool CanBeActivatedBy(Player controller)
        {
            return serverGame.uiCtrl.DebugMode || base.CanBeActivatedBy(controller);
        }

        public void PushToStack(ServerPlayer controller, ActivationContext context)
        {
            EffectsController.PushToStack(this, controller, context);
        }

        public void PushedToStack(ServerGame game, ServerPlayer ctrl)
        {
            TimesUsedThisRound++;
            TimesUsedThisTurn++;
            serverGame = game;
            Controller = ctrl;
            ctrl.ServerNotifier.NotifyEffectActivated(this);
        }

        public void StartResolution(ActivationContext context)
        {
            Debug.Log($"Resolving effect {EffectIndex} of {Source.CardName} in context {context}");
            serverGame.CurrEffect = this;

            //set context parameters
            CurrActivationContext = context;
            X = context.X ?? 0;
            if(context.Targets != null) Targets.AddRange(context.Targets);

            //notify relevant to this effect starting
            ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
            ServerController.ServerNotifier.EffectResolving(this);

            //resolve the effect if possible
            if (Negated) EffectImpossible();
            else ResolveSubeffect(context.StartIndex);
        }

        public bool ResolveNextSubeffect()
        {
            return ResolveSubeffect(SubeffectIndex + 1);
        }

        public bool ResolveSubeffect(int index)
        {
            if (index >= ServerSubeffects.Length)
            {
                FinishResolution();
                return true;
            }

            Debug.Log($"Resolving subeffect of type {ServerSubeffects[index].GetType()}");
            SubeffectIndex = index;
            ServerController.ServerNotifier.NotifyEffectX(Source, EffectIndex, X);
            return ServerSubeffects[index].Resolve();
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
            Targets.Clear();
            Rest.Clear();
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
            Debug.Log($"Effect of {Source.CardName} is being declared impossible");
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

        public void DeclineAnotherTarget()
        {
            OnImpossible?.OnImpossible();
        }

        public override void AddTarget(GameCard card)
        {
            base.AddTarget(card);
            serverGame.ServerPlayers[card.ControllerIndex].ServerNotifier.SetTarget(Source, EffectIndex, card);
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