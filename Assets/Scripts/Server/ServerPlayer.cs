using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Effects;
using KompasServer.Networking;
using System.Net.Sockets;
using UnityEngine;

namespace KompasServer.GameCore
{
    public class ServerPlayer : Player
    {
        public ServerPlayer ServerEnemy;
        public ServerGame serverGame;
        public ServerNetworkController ServerNetworkCtrl;
        public ServerNotifier ServerNotifier;

        public override Player Enemy => ServerEnemy;

        public override int Pips
        {
            get => base.Pips;
            set
            {
                base.Pips = value;
                ServerNotifier.NotifySetPips(Pips);
            }
        }

        public override GameCard Avatar
        {
            get => base.Avatar;
            set
            {
                base.Avatar = value;
                ServerNotifier.SetFriendlyAvatar(value.CardName, value.ID);
            }
        }

        public override void SetInfo(TcpClient tcpClient, int index)
        {
            base.SetInfo(tcpClient, index);

            ServerNetworkCtrl.SetInfo(tcpClient);
        }

        //If the player tries to do something, it goes here to check if it's ok, then do it if it is ok.
        #region Player Control Methods
        /// <summary>
        /// x and y here are from playerIndex's perspective
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryAugment(GameCard aug, int x, int y)
        {
            if (serverGame.ValidAugment(aug, x, y, this)) aug.Play(x, y, this, payCost: true);
            else ServerNotifier.NotifyPutBack();

            serverGame.EffectsController.CheckForResponse();
        }

        public void TryPlay(GameCard card, int x, int y)
        {
            if (serverGame.ValidBoardPlay(card, x, y, this)) card.Play(x, y, this, payCost: true);
            else ServerNotifier.NotifyPutBack();

            serverGame.EffectsController.CheckForResponse();
        }

        public void TryMove(GameCard toMove, int x, int y)
        {
            //if it's not a valid place to do, put the cards back
            if (serverGame.ValidMove(toMove, x, y)) toMove.Move(x, y, true);
            else ServerNotifier.NotifyPutBack();

            serverGame.EffectsController.CheckForResponse();
        }

        /// <summary>
        /// If it is a valid action to take, activates the effect, adding it to the stack and suchlike
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="controller"></param>
        public void TryActivateEffect(ServerEffect effect)
        {
            Debug.Log($"Player {index} trying to activate effect of {effect?.Source?.CardName}");
            if (effect.CanBeActivatedBy(this))
            {
                serverGame.EffectsController.PushToStack(effect, this, new ActivationContext());
                serverGame.EffectsController.CheckForResponse();
            }
        }

        public void TryAttack(GameCard attacker, GameCard defender)
        {
            ServerNotifier.NotifyBothPutBack();

            if (serverGame.ValidAttack(attacker, defender, this))
                serverGame.Attack(attacker, defender, this, true);

            serverGame.EffectsController.CheckForResponse();
        }

        public void TryEndTurn()
        {
            if (serverGame.EffectsController.CurrStackEntry == null &&
                serverGame.TurnPlayer == this) 
                serverGame.SwitchTurn();
        }
        #endregion Player Control Methods
    }
}