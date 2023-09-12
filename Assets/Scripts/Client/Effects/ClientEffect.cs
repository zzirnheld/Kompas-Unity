﻿using UnityEngine;
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
			get => ClientController;
			set => ClientController = value as ClientPlayer;
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

		public override IResolutionContext CurrentResolutionContext
		{
			get
			{
				if (base.CurrentResolutionContext == null) CurrentResolutionContext = KompasCore.Effects.ResolutionContext.PlayerTrigger(this, Game);
				return base.CurrentResolutionContext;
			}
			protected set => base.CurrentResolutionContext = value;
		}

		public string StackableBlurb => blurb;

		public void SetInfo(GameCard thisCard, ClientGame clientGame, int effectIndex, ClientPlayer owner)
		{
			this.ClientGame = clientGame;
			base.SetInfo(thisCard, effectIndex, owner);
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
		public void Activated(ResolutionContext context = default)
		{
			TimesUsedThisTurn++;
			TimesUsedThisRound++;
			TimesUsedThisStack++;

			ClientGame.EffectActivated(this);
			ClientGame.clientEffectsCtrl.Add(this, context);
		}

		public void StartResolution(TriggeringEventContext context)
		{
			ClientGame.clientUIController.currentStateUIController.ResolvingEffect(Source.CardName, blurb);
			CardTargets.Clear();

			//in case any cards are still showing targets from the last effect, which they will if this happens after another effect in the stack.
			//TODO move this behavior to a "effect end" packet and stuff?
			ClientGame.ShowNoTargets();
		}
	}
}