﻿using KompasClient.Effects;
using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.Effects;
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
                ClientGame.clientUICtrl.Leyload = Game.Leyload;
                if (cardCtrl != null)
                {
                    cardCtrl.ShowForCardType(CardType, ClientCameraController.Main.Zoomed);
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
                if (cardCtrl != null) cardCtrl.SetRotation();
            }
        }
        public override Player Controller
        {
            get => ClientController;
            set => ClientController = value as ClientPlayer;
        }

        public ClientPlayer ClientOwner { get; private set; }
        public override Player Owner
        {
            get => ClientOwner;
            protected set => ClientOwner = value as ClientPlayer;
        }

        public ClientEffect[] ClientEffects { get; private set; }
        public override IEnumerable<Effect> Effects => ClientEffects;
        public override bool IsAvatar => false;
        public ClientCardMouseController mouseCtrl;
        public ClientCardController clientCardCtrl;

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

        public override bool Remove(IStackable stackSrc = null)
        {
            ClientGame.MarkCardDirty(this);
            return base.Remove(stackSrc);
        }

        public void SetInitialCardInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
        {
            ClientGame = game;
            ClientController = ClientOwner = owner;
            ClientEffects = effects;
            mouseCtrl.clientGame = game;
            base.SetCardInfo(serializedCard, id);
            int i = 0;
            foreach (var eff in effects) eff.SetInfo(this, game, i++, owner);
        }

        public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
        {
            base.SetN(n, stackSrc, notify);
            if (ClientGame?.clientUICtrl.ShownCard == this)
                ClientGame?.clientUICtrl.Refresh();
        }

        /// <summary>
        /// Updates the clientCardCtrl to show the little revealed eye iff the card:<br/>
        /// - is known to enemy<br/>
        /// - is in an otherwise hidden location<br/>
        /// - is controlled by an enemy<br/>
        /// </summary>
        private void UpdateRevealed()
        {
            if (clientCardCtrl != null)
            {
                clientCardCtrl.Revealed = KnownToEnemy && InHiddenLocation && !Owner.Friendly;
            }
        }
    }
}