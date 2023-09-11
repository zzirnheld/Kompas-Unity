﻿using KompasClient.GameCore;
using KompasCore.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

using static KompasClient.UI.ConnectionUIController;

namespace KompasClient.Networking
{
	public class ClientNetworkController : NetworkController
	{
		public ClientGame ClientGame;
		private bool connecting = false;

		public async void Connect(string ip)
		{
			if (connecting) return;

			connecting = true;
			var address = IPAddress.Parse(ip);
			tcpClient = new System.Net.Sockets.TcpClient();
			try
			{
				await tcpClient.ConnectAsync(address, port);
			}
			catch (SocketException e)
			{
				Debug.LogError($"Failed to connect to {ip}. Stack trace:\n{e.StackTrace}");
				ClientGame.clientUIController.connectionUIController.Show(ConnectionState.ChooseServer);
			}
			
			if (tcpClient.Connected)
			{
				Debug.Log("Connected");
				ClientGame.clientUIController.connectionUIController.Show(ConnectionState.WaitingForPlayer);
			}
			else ClientGame.clientUIController.connectionUIController.Show(ConnectionState.ChooseServer);

			connecting = false;
		}

		protected override void Update()
		{
			base.Update();
			if (connecting) return;
			if (packets.Count != 0) ProcessPacket(packets.Dequeue());

			//TODO: if !tcpClient.connected
		}

		private static readonly Dictionary<string, System.Type> jsonTypes = new()
		{
			//game start
			{ Packet.GetDeck, typeof(GetDeckClientPacket)},
			{ Packet.DeckAccepted, typeof(DeckAcceptedClientPacket)},
			{ Packet.SetAvatar, typeof(SetAvatarClientPacket)},
			{ Packet.SetFirstTurnPlayer, typeof(SetFirstPlayerClientPacket)},
			//game end
			{ Packet.GameEnd, typeof(GameEndClientPacket)},
			//gamestate
			{ Packet.SetLeyload, typeof(SetLeyloadClientPacket)},
			{ Packet.SetTurnPlayer, typeof(SetTurnPlayerClientPacket)},
			{ Packet.PutCardsBack, typeof(PutCardsBackClientPacket)},
			{ Packet.AttackStarted, typeof(AttackStartedClientPacket)},
			{ Packet.HandSizeToStack, typeof(HandSizeToStackClientPacket)},
			{ Packet.ChooseHandSize, typeof(GetHandSizeChoicesClientPacket)},
			{ Packet.SetDeckCount, typeof(SetDeckCountClientPacket)},
			//card addition/deletion
			{ Packet.AddCard, typeof(AddCardClientPacket)},
			{ Packet.DeleteCard, typeof(DeleteCardClientPacket)},
			{ Packet.ChangeEnemyHandCount, typeof(ChangeEnemyHandCountClientPacket)},
			//card movement
			{ Packet.KnownToEnemy, typeof(UpdateKnownToEnemyClientPacket)},
			//public areas
			{ Packet.PlayCard, typeof(PlayCardClientPacket)},
			{ Packet.AttachCard, typeof(AttachCardClientPacket)},
			{ Packet.MoveCard, typeof(MoveCardClientPacket)},
			{ Packet.DiscardCard, typeof(DiscardCardClientPacket)},
			{ Packet.AnnihilateCard, typeof(AnnihilateCardClientPacket)},
			//private areas
			{ Packet.RehandCard, typeof(RehandCardClientPacket)},
			{ Packet.TopdeckCard, typeof(TopdeckCardClientPacket)},
			{ Packet.ReshuffleCard, typeof(ReshuffleCardClientPacket)},
			{ Packet.BottomdeckCard, typeof(BottomdeckCardClientPacket)},
			//stats
			{ Packet.UpdateCardNumericStats, typeof(ChangeCardNumericStatsClientPacket)},
			{ Packet.NegateCard, typeof(NegateCardClientPacket)},
			{ Packet.ActivateCard, typeof(ActivateCardClientPacket)},
			{ Packet.ChangeCardController, typeof(ChangeCardControllerClientPacket)},
			{ Packet.SetPips, typeof(SetPipsClientPacket)},
			{ Packet.AttacksThisTurn, typeof(AttacksThisTurnClientPacket)},
			{ Packet.SpacesMoved, typeof(SpacesMovedClientPacket)},
			//effects
			//targeting
			{ Packet.GetCardTarget, typeof(GetCardTargetClientPacket)},
			{ Packet.GetSpaceTarget, typeof(GetSpaceTargetClientPacket)},
			//other
			{ Packet.GetEffectOption, typeof(GetEffectOptionClientPacket)},
			{ Packet.EffectResolving, typeof(EffectResolvingClientPacket)},
			{ Packet.EffectActivated, typeof(EffectActivatedClientPacket)},
			{ Packet.RemoveStackEntry, typeof(RemoveStackEntryClientPacket)},
			{ Packet.SetEffectsX, typeof(SetEffectsXClientPacket)},
			{ Packet.PlayerChooseX, typeof(GetPlayerChooseXClientPacket)},
			{ Packet.TargetAccepted, typeof(TargetAcceptedClientPacket)},
			{ Packet.AddTarget, typeof(AddTargetClientPacket)},
			{ Packet.RemoveTarget, typeof(RemoveTargetClientPacket)},
			{ Packet.ToggleDecliningTarget, typeof(ToggleDecliningTargetClientPacket)},
			{ Packet.StackEmpty, typeof(StackEmptyClientPacket)},
			{ Packet.EffectImpossible, typeof(EffectImpossibleClientPacket)},
			{ Packet.OptionalTrigger, typeof(OptionalTriggerClientPacket)},
			{ Packet.ToggleAllowResponses, typeof(ToggleAllowResponsesClientPacket)},
			{ Packet.GetTriggerOrder, typeof(GetTriggerOrderClientPacket)},
			{ Packet.EditCardLink, typeof(EditCardLinkClientPacket)},
		}; //TODO a unit test that asserts that all types in this map extend IClientOrderPacket

		private IClientOrderPacket FromJson(string command, string json)
		{
			if (!jsonTypes.ContainsKey(command)) throw new System.ArgumentException($"Unrecognized command {command} in packet sent to client");

			return JsonConvert.DeserializeObject(json, jsonTypes[command]) as IClientOrderPacket;
		}

		public override Task ProcessPacket((string command, string json) packetInfo)
		{
			if (packetInfo.command == Packet.Invalid)
			{
				Debug.LogError("Invalid packet");
				return Task.CompletedTask;
			}

			var p = FromJson(packetInfo.command, packetInfo.json);
			//Debug.Log($"Parsing packet {p}");
			p.Execute(ClientGame);

			//clean up any visual differences after the latest packet.
			//TODO make this more efficient, probably with dirty lists
			ClientGame.Refresh();

			return Task.CompletedTask;
		}
	}
}