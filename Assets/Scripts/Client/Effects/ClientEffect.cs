using UnityEngine;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasClient.GameCore;

namespace KompasClient.Effects
{
    public class ClientEffect : Effect
    {
        public ClientPlayer ClientController;
        public override Player Controller
        {
            get { return ClientController; }
            set { ClientController = value as ClientPlayer; }
        }
        public ClientGame ClientGame { get; }
        public DummySubeffect[] DummySubeffects { get; }
        public ClientTrigger ClientTrigger { get; }

        public override Subeffect[] Subeffects => DummySubeffects;
        public override Trigger Trigger => ClientTrigger;


        public ClientEffect(SerializableEffect se, GameCard thisCard, ClientGame clientGame, int effectIndex, ClientPlayer owner)
            : base(se.activationRestriction ?? new ActivationRestriction(), thisCard, se.blurb, effectIndex, owner)
        {
            this.ClientGame = clientGame;
            DummySubeffects = new DummySubeffect[se.subeffects.Length];

            if (!string.IsNullOrEmpty(se.trigger))
            {
                try
                {
                    ClientTrigger = ClientTrigger.FromJson(se.triggerCondition, se.trigger, this);
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
                    DummySubeffects[i] = DummySubeffect.FromJson(se.subeffects[i], this, i);
                }
                catch (System.ArgumentException)
                {
                    Debug.LogError($"Failed to load subeffect from json {se.subeffects[i]}");
                }
            }
        }

        public override void AddTarget(GameCard card)
        {
            base.AddTarget(card);
            card.cardCtrl.ShowCurrentTarget(true);
        }

        public override void RemoveTarget(GameCard card)
        {
            base.RemoveTarget(card);
            card.cardCtrl.ShowCurrentTarget(false);
        }

        public void Activated()
        {
            TimesUsedThisTurn++;
            TimesUsedThisRound++;
            TimesUsedThisStack++;

            ClientGame.EffectActivated(this);
        }

        public override void StartResolution(ActivationContext context)
        {
            ClientGame.clientUICtrl.SetCurrState($"Resolving Effect of {Source.CardName}", $"{Blurb}");
            TargetsList.Clear();

            //in case any cards are still showing targets from the last effect, which they will if this happens after another effect in the stack.
            //TODO move this behavior to a "effect end" packet and stuff?
            ClientGame.ShowNoTargets();
        }
    }
}