using UnityEngine;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasClient.GameCore;
using KompasCore.GameCore;

namespace KompasClient.Effects
{
    public class ClientEffect : Effect, IClientStackable
    {
        public ClientPlayer ClientController;
        public override Player Controller
        {
            get { return ClientController; }
            set { ClientController = value as ClientPlayer; }
        }
        public ClientGame ClientGame { get; private set; }
        public override Game Game => ClientGame;
        public DummySubeffect[] DummySubeffects { get; }
        public ClientTrigger ClientTrigger { get; private set; }

        public override Subeffect[] Subeffects => DummySubeffects;
        public override Trigger Trigger => ClientTrigger;

        public Sprite PrimarySprite => Source.SimpleSprite;
        public CardController PrimaryCardController => Source.CardController;

        public Sprite SecondarySprite => default;
        public CardController SecondaryCardController => default;

        public string StackableBlurb => blurb;

        public void SetInfo(GameCard thisCard, ClientGame clientGame, int effectIndex, ClientPlayer owner)
        {
            base.SetInfo(thisCard, effectIndex, owner);
            this.ClientGame = clientGame;
            if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
                ClientTrigger = new ClientTrigger(triggerData, this);
        }

        public override void AddTarget(GameCard card)
        {
            base.AddTarget(card);
            card.CardController.gameCardViewController.Refresh();
        }

        public override void RemoveTarget(GameCard card)
        {
            base.RemoveTarget(card);
            card.CardController.gameCardViewController.Refresh();
        }

        //TODO eventually make client aware of activation contexts
        public void Activated(ActivationContext context = default)
        {
            TimesUsedThisTurn++;
            TimesUsedThisRound++;
            TimesUsedThisStack++;

            ClientGame.EffectActivated(this);
            ClientGame.clientEffectsCtrl.Add(this, context);
        }

        public void StartResolution(ActivationContext context)
        {
            ClientGame.clientUIController.SetCurrState($"Resolving Effect of {Source.CardName}", $"{blurb}");
            cardTargets.Clear();

            //in case any cards are still showing targets from the last effect, which they will if this happens after another effect in the stack.
            //TODO move this behavior to a "effect end" packet and stuff?
            ClientGame.ShowNoTargets();
        }
    }
}