using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGameCard : GameCard
{
    public ClientGame ClientGame { get; private set; }
    public override Game Game => ClientGame;

    private ClientPlayer clientController;
    public ClientPlayer ClientController
    {
        get => clientController;
        set
        {
            clientController = value;
            cardCtrl.SetRotation();
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

    public void SetInfo(SerializableCard serializedCard, ClientGame game, ClientPlayer owner, ClientEffect[] effects, int id)
    {
        base.SetInfo(serializedCard, id);
        ClientGame = game;
        ClientController = ClientOwner = owner;
        ClientEffects = effects;
    }
}
