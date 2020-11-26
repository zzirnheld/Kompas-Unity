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
            set
            {
                base.Location = value;
                ClientGame.clientUICtrl.Leyload = Game.Leyload;
                if(cardCtrl != null) cardCtrl.ShowForCardType(CardType, ClientCameraController.Main.Zoomed);
            }
        }

        private ClientPlayer clientController;
        public ClientPlayer ClientController
        {
            get => clientController;
            set
            {
                clientController = value;
                if(cardCtrl != null) cardCtrl.SetRotation();
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
        public override IEnumerable<Effect> Effects => ClientEffects;
        public override bool IsAvatar => false;

        public virtual void SetInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
        {
            base.SetInfo(serializedCard, id);
            ClientGame = game;
            ClientController = ClientOwner = owner;
            ClientEffects = effects;
        }

        public override void ResetCard()
        {
            base.ResetCard();

            if(cardCtrl != null) cardCtrl.ShowForCardType(CardType, ClientCameraController.Main.Zoomed);
        }

        public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
        {
            base.SetN(n, stackSrc, notify);
            if (ClientGame.clientUICtrl.ShownCard == this)
                ClientGame.clientUICtrl.RefreshShownCardInfo();
        }
    }
}