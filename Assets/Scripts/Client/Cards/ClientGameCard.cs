using KompasClient.Effects;
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
                    clientCardCtrl.Revealed = KnownToEnemy && InHiddenLocation;
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
                clientCardCtrl.Revealed = KnownToEnemy && InHiddenLocation;
            }
        }

        public override void Remove(IStackable stackSrc = null)
        {
            ClientGame.MarkCardDirty(this);
            base.Remove(stackSrc);
        }

        public void SetInitialCardInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
        {
            base.SetCardInfo(serializedCard, id);
            ClientGame = game;
            ClientController = ClientOwner = owner;
            ClientEffects = effects;
            mouseCtrl.ClientGame = game;
            int i = 0;
            foreach (var eff in effects) eff.SetInfo(this, game, i++, owner);
        }

        public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
        {
            base.SetN(n, stackSrc, notify);
            if (ClientGame?.clientUICtrl.ShownCard == this)
                ClientGame?.clientUICtrl.Refresh();
        }
    }
}