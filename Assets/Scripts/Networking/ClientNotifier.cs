using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;

public class ClientNotifier : MonoBehaviour
{
    public ClientNetworkController clientNetworkCtrl;

    private void Send(Packet packet)
    {
        clientNetworkCtrl.SendPacket(packet);
    }

    #region Request Actions
    public void RequestPlay(Card card, int toX, int toY)
    {
        Packet packet;
        if (card is AugmentCard) packet = new Packet(Packet.Command.Augment, card, toX, toY);
        else packet = new Packet(Packet.Command.Play, card, toX, toY);
        Send(packet);
    }

    public void RequestMove(Card card, int toX, int toY)
    {
        Packet packet = new Packet(Packet.Command.Move, card, toX, toY);
        Send(packet);
    }

    public void RequestAttack(Card card, int toX, int toY)
    {
        Packet packet = new Packet(Packet.Command.Attack, card, toX, toY);
        Send(packet);
    }

    public void RequestTopdeck(Card card)
    {
        Packet packet = new Packet(Packet.Command.Topdeck, card);
        Send(packet);
    }

    public void RequestDiscard(Card card)
    {
        Packet packet = new Packet(Packet.Command.Discard, card);
        Send(packet);
    }

    public void RequestRehand(Card card)
    {
        Packet packet = new Packet(Packet.Command.Rehand, card);
        Send(packet);
    }

    public void RequestDecklistImport(string decklist)
    {
        Debug.Log("Requesting Deck import of \"" + decklist + "\"");
        string[] cardNames = decklist.Split('\n');
        Packet packet = new Packet(Packet.Command.SetDeck)
        {
            stringArg = decklist
        };
        Send(packet);
    }

    public void RequestDraw()
    {
        Packet packet = new Packet(Packet.Command.Draw);
        Send(packet);
    }

    public void RequestSetNESW(Card card, int n, int e, int s, int w)
    {
        Packet packet = new Packet(Packet.Command.SetNESW, card, n, e, s, w);
        Send(packet);
    }

    public void RequestUpdatePips(int num)
    {
        Packet packet = new Packet(Packet.Command.SetPips, num);
        Send(packet);
        Debug.Log("requesting updating pips to " + num);
    }

    public void RequestEndTurn()
    {
        Packet packet = new Packet(Packet.Command.EndTurn);
        Send(packet);
    }

    public void RequestTarget(Card card)
    {
        Debug.Log("Requesting target " + card.CardName);
        Packet packet = new Packet(Packet.Command.Target, card);
        Send(packet);
    }

    public void RequestResolveEffect(Card card, int index)
    {
        Debug.Log("Requesting effect of " + card.CardName + " number" + index);
        Packet packet = new Packet(Packet.Command.TestTargetEffect, card, index);
        Send(packet);
    }

    public void RequestSetX(int x)
    {
        Debug.Log("Requesting to set X to " + x);
        Packet packet = new Packet(Packet.Command.PlayerSetX, x);
        Send(packet);
    }

    public void DeclineAnotherTarget()
    {
        Debug.Log("Declining to select another target");
        Packet packet = new Packet(Packet.Command.DeclineAnotherTarget);
        Send(packet);
    }

    public void RequestSpaceTarget(int x, int y)
    {
        Debug.Log("Requesting a space target of " + x + ", " + y);
        Packet packet = new Packet(Packet.Command.SpaceTarget, x, y);
        Send(packet);
    }
    #endregion
}
