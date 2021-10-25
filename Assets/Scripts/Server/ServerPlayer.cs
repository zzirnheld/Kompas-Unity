﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Effects;
using KompasServer.Networking;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.GameCore
{
    public class ServerPlayer : Player
    {
        public ServerPlayer ServerEnemy;
        public ServerGame serverGame;
        public ServerNetworkController ServerNetworkCtrl;
        public ServerNotifier ServerNotifier;
        public ServerAwaiter serverAwaiter;

        public override Player Enemy => ServerEnemy;
        public override bool Friendly => false;

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
                ServerNotifier.SetFriendlyAvatar(value.BaseJson, value.ID);
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
        public async Task TryAugment(GameCard aug, Space space)
        {
            if (serverGame.ValidAugment(aug, space, this))
            {
                aug.Play(space, this, payCost: true);
                await serverGame.EffectsController.CheckForResponse();
            }
            else ServerNotifier.NotifyPutBack();
        }

        public async Task TryPlay(GameCard card, Space space)
        {
            if (serverGame.ValidBoardPlay(card, space, this))
            {
                card.Play(space, this, payCost: true);
                await serverGame.EffectsController.CheckForResponse();
            }
            else ServerNotifier.NotifyPutBack();
        }

        public async Task TryMove(GameCard toMove, Space space)
        {
            //if it's not a valid place to do, put the cards back
            if (serverGame.ValidMove(toMove, space, this))
            {
                toMove.Move(space, true);
                await serverGame.EffectsController.CheckForResponse();
            }
            else ServerNotifier.NotifyPutBack();
        }

        /// <summary>
        /// If it is a valid action to take, activates the effect, adding it to the stack and suchlike
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="controller"></param>
        public async Task TryActivateEffect(ServerEffect effect)
        {
            Debug.Log($"Player {index} trying to activate effect of {effect?.Source?.CardName}");
            if (effect.CanBeActivatedBy(this))
            {
                serverGame.EffectsController.PushToStack(effect, this, new ActivationContext());
                await serverGame.EffectsController.CheckForResponse();
            }
        }

        public async Task TryAttack(GameCard attacker, GameCard defender)
        {
            ServerNotifier.NotifyBothPutBack();

            if (serverGame.ValidAttack(attacker, defender, this))
            {
                serverGame.Attack(attacker, defender, this, manual: true);
                await serverGame.EffectsController.CheckForResponse();
            }
        }

        public async Task TryEndTurn()
        {
            if (serverGame.NothingHappening && serverGame.TurnPlayer == this) 
                await serverGame.SwitchTurn();
        }
        #endregion Player Control Methods
    }
}