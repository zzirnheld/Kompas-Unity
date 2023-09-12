using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Effects;
using KompasCore.Exceptions;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.GameCore.Extensions;
using KompasServer.Networking;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.GameCore
{
	public class ServerPlayer : Player
	{
		public ServerPlayer enemy;
		public ServerGame game;
		public ServerNetworkController networkController;
		public ServerNotifier notifier;
		public ServerAwaiter awaiter;

		public override Player Enemy => enemy;
		public override bool Friendly => false;
		public override Game Game => game;

		public override int Pips
		{
			get => base.Pips;
			set
			{
				base.Pips = value;
				notifier.NotifySetPips(Pips);
			}
		}

		public override GameCard Avatar
		{
			get => base.Avatar;
			set
			{
				base.Avatar = value;
				notifier.SetFriendlyAvatar(value.BaseJson, value.ID);
			}
		}

		public override void SetInfo(TcpClient tcpClient, int index)
		{
			base.SetInfo(tcpClient, index);

			networkController.SetInfo(tcpClient);
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
			try
			{
				if (game.IsValidNormalAttach(aug, space, this))
				{
					aug.Play(space, this, payCost: true);
					await game.effectsController.CheckForResponse();
				}
				else notifier.NotifyPutBack();
			}
			catch (KompasException ke)
			{
				Debug.LogError(ke);
				notifier.NotifyPutBack();
			}
		}

		public async Task TryPlay(GameCard card, Space space)
		{
			try
			{
				if (game.IsValidNormalPlay(card, space, this))
				{
					card.Play(space, this, payCost: true);
					await game.effectsController.CheckForResponse();
				}
				else
				{
					Debug.LogWarning($"Player {index} attempted an invalid play of {card} to {space}.");
					notifier.NotifyPutBack();
				}
			}
			catch (KompasException ke)
			{
				Debug.LogError($"Player {index} attempted an invalid play of {card} to {space}. Resulting exception:\n{ke}");
				notifier.NotifyPutBack();
			}
		}

		public async Task TryMove(GameCard toMove, Space space)
		{
			//if it's not a valid place to do, put the cards back
			try
			{
				if (game.IsValidNormalMove(toMove, space, this))
				{
					toMove.Move(space, true);
					await game.effectsController.CheckForResponse();
				}
				else notifier.NotifyPutBack();
			}
			catch (KompasException ke)
			{
				Debug.LogError(ke);
				notifier.NotifyPutBack();
			}
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
				var context = ResolutionContext.PlayerTrigger(effect, game);
				game.effectsController.PushToStack(effect, this, context);
				await game.effectsController.CheckForResponse();
			}
		}

		public async Task TryAttack(GameCard attacker, GameCard defender)
		{
			notifier.NotifyBothPutBack();

			if (game.IsValidNormalAttack(attacker, defender, this))
			{
				game.Attack(attacker, defender, this, stackSrc: default, manual: true);
				await game.effectsController.CheckForResponse();
			}
		}

		public async Task TryEndTurn()
		{
			if (game.NothingHappening && game.TurnPlayer == this)
				await game.SwitchTurn();
		}
		#endregion Player Control Methods

		public void Lose() => game.Lose(this);
	}
}