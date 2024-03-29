﻿using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Helpers;
using KompasCore.GameCore;
using System.Collections.Generic;

namespace KompasClient.Cards
{
	public class ClientGameCard : GameCard
	{
		public ClientGame ClientGame { get; protected set; }
		public override Game Game => ClientGame;

		public override CardLocation Location
		{
			get => base.Location;
			protected set
			{
				base.Location = value;
				ClientGame.clientUIController.Leyload = Game.Leyload;
				if (CardController != null)
				{
					CardController.gameCardViewController.Refresh();
					UpdateRevealed();
				}
			}
		}

		private ClientPlayer clientController;
		public ClientPlayer ClientController
		{
			get => clientController;
			set
			{
				clientController = value;
				if (CardController != null) CardController.gameCardViewController.Refresh();
			}
		}
		public override Player Controller
		{
			get => ClientController;
			set => ClientController = value as ClientPlayer;
		}

		public ClientPlayer ClientOwner { get; private set; }
		public override Player Owner => ClientOwner;

		public ClientEffect[] ClientEffects { get; private set; }
		public override IReadOnlyCollection<Effect> Effects => ClientEffects;
		public override bool IsAvatar => false;
		public ClientCardController ClientCardController { get; private set; }
		public override CardController CardController => ClientCardController;

		private bool knownToEnemy = false;
		public override bool KnownToEnemy
		{
			get => knownToEnemy;
			set
			{
				knownToEnemy = value;
				UpdateRevealed();
			}
		}

		protected ClientGameCard(int id, ClientPlayer owner, ClientCardController clientCardController)
			: base(id)
		{
			owner.game.AddCard(this);

			ClientCardController = clientCardController;
			clientCardController.ClientCard = this;

			ClientGame = owner.game;
			ClientController = ClientOwner = owner;
		}

		public ClientGameCard(SerializableCard serializedCard, int id, ClientPlayer owner, ClientEffect[] effects, ClientCardController clientCardController)
			: base (serializedCard, id, owner.game)
		{
			owner.game.AddCard(this);

			ClientCardController = clientCardController;
			clientCardController.ClientCard = this;

			ClientGame = owner.game;
			ClientController = ClientOwner = owner;
			ClientEffects = effects;
			foreach (var (index, eff) in effects.Enumerate()) eff.SetInfo(this, ClientGame, index, owner);

			clientCardController.gameCardViewController.Focus(this);
		}

		//Factory, because apparently c# doesn't let you pass constructors?
		public static ClientGameCard Create(SerializableCard serializedCard, int id, ClientPlayer owner, ClientEffect[] effects, ClientCardController clientCardController)
			=> new ClientGameCard(serializedCard, id, owner, effects, clientCardController);

		public override bool Remove(IStackable stackSrc = null)
		{
			ClientGame.MarkCardDirty(this);
			return base.Remove(stackSrc);
		}

		public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
		{
			base.SetN(n, stackSrc, notify);
			ClientGame?.clientUIController.CardViewController.Refresh();
		}

		/// <summary>
		/// Updates the clientCardCtrl to show the little revealed eye iff the card:<br/>
		/// - is known to enemy<br/>
		/// - is in an otherwise hidden location<br/>
		/// - is controlled by an enemy<br/>
		/// </summary>
		private void UpdateRevealed()
		{
			if (ClientCardController != null)
			{
				ClientCardController.Revealed = KnownToEnemy && InHiddenLocation && !Owner.Friendly;
			}
		}
	}
}