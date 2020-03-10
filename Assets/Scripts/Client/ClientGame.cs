using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using KompasNetworking;

public class ClientGame : Game {

    public static ClientGame mainClientGame;

    public GameObject AvatarPrefab;

    public override Player[] Players => ClientPlayers;
    public ClientPlayer[] ClientPlayers;

    private bool friendlyTurn;

    public HandController friendlyHandCtrl;
    public DeckController friendlyDeckCtrl;
    public DiscardController friendlyDiscardCtrl;

    public GameObject friendlyHandObj;
    public GameObject friendlyDeckObj;
    public GameObject friendlyDiscardObj;

    public HandController enemyHandCtrl;
    public DeckController enemyDeckCtrl;
    public DiscardController enemyDiscardCtrl;

    public GameObject enemyHandObj;
    public GameObject enemyDeckObj;
    public GameObject enemyDiscardObj;

    public ClientNetworkController clientNetworkCtrl;
    public ClientNotifier clientNotifier;
    public ClientUIController clientUICtrl;

    //targeting
    public CardRestriction CurrCardRestriction;
    public SpaceRestriction CurrSpaceRestriction;

    private void Start()
    {
        mainGame = this;
        mainClientGame = this;
        SubeffectFactory = new ClientSubeffectFactory();
    }

    //game mechanics
    #region setting pips
    public void SetFriendlyPips(int num)
    {
        Players[0].pips = num;
        uiCtrl.UpdateFriendlyPips(num);
    }

    public void SetEnemyPips(int num)
    {
        Players[1].pips = num;
        uiCtrl.UpdateEnemyPips(num);
    }
    #endregion

    public void SetAvatar(int player, string avatarName, int avatarID)
    {
        if (player >= 2) throw new System.ArgumentException();

        Player owner = Players[player];
        AvatarCard avatar = CardRepo.InstantiateAvatar(avatarName, AvatarPrefab, this, owner, avatarID);
        owner.Avatar = avatar;
        Play(avatar, player * 6, player * 6, owner);
    }

    public void Delete(Card card)
    {
        Destroy(card.gameObject);
        //probably destroy and not set inactive because a card that is deleted and played again will just be created anew
        //card.gameObject.SetActive(false);
    }

    //requesting
    public void RequestMove(Card card, int toX, int toY)
    {
        clientNotifier.RequestMove(card, toX, toY);
    }

    public void RequestPlay(Card card, int toX, int toY)
    {
        clientNotifier.RequestPlay(card, toX, toY);
    }

    public void TargetCard(Card card)
    {
        if (clientNetworkCtrl.Restriction == null) return;

        if(CurrCardRestriction == null)
        {
            Debug.LogError($"Tried to target {card.CardName} while last restriction is null or not card restriction");
        }

        //if the player is currently looking for a target on the board,
        if (targetMode == TargetMode.BoardTarget || targetMode == TargetMode.HandTarget)
        {
            //check if the target fits the restriction, according to us
            if (CurrCardRestriction.Evaluate(card, clientNetworkCtrl.X))
            {
                //if it fits the restriction, send the proposed target to the server
                clientNotifier.RequestTarget(card);

                //and change the game's target mode TODO should this do this
                targetMode = TargetMode.OnHold;
            }
        }
        else
        {
            Debug.LogError($"Tried to target while in not understood target mode {targetMode}");
        }
    }


}
