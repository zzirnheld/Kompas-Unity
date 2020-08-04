using KompasClient.GameCore;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    [System.Serializable]
    public abstract class Packet
    {
        public const string Invalid = "Invalid Command";

        //game start
        public const string GetDeck = "Get Deck";
        public const string SetDeck = "Set Deck";
        public const string DeckAccepted = "Deck Accepted";
        public const string SetAvatar = "Set Avatar";
        public const string SetFirstTurnPlayer = "Set First Turn Player";

        //player action: things the player initiates
        public const string PlayAction = "Player Play Action";
        public const string AugmentAction = "Player Augment Action";
        public const string MoveAction = "Player Move Action";
        public const string AttackAction = "Player Attack Action";
        public const string EndTurnAction = "Player End Turn Action";

        //effect commands
        public const string CardTargetChosen = "Card Target Chosen";
        public const string SpaceTargetChosen = "Space Target Chosen";
        public const string XSelectionChosen = "X Value Chosen";
        public const string DeclineAnotherTarget = "Decline Another Target";
        public const string ListChoicesChosen = "List Choices Chosen";
        public const string OptionalTriggerResponse = "Optional Trigger Answered";
        public const string ChooseEffectOption = "Choose Effect Option";

        /*
        public enum Command
        {
            Invalid = -1,
            //game start procedures
            GetDeck = 0,
            SetDeck = 1,
            DeckAccepted = 2,
            SetAvatar = 10,
            SetFirstTurnPlayer = 11,
            //basic player commands
            PlayAction = 100,
            Move = 102,
            Attack = 103,
            Attach = 104,
            AddAsEnemyAndAttach = 105,
            EndTurn = 150,
            Leyload = 151,
            //move cards around
            Topdeck = 200,
            Discard = 201,
            Rehand = 202,
            Reshuffle = 203,
            AddAsFriendly = 204,
            AddAsEnemy = 205,
            Bottomdeck = 206,
            Draw = 207,
            Delete = 208,
            Annihilate = 209,
            Play = 250,
            Augment = 251,
            //card properties
            SetNESW = 301,
            Negate = 302,
            SetSpellStats = 303,
            Activate = 304,
            Deactivate = 305,
            ChangeControl = 306,
            SetN = 350,
            SetE = 351,
            SetS = 352,
            SetW = 353,
            SetC = 354,
            SetA = 355,
            //change numbers of cards that you see of your opponent
            IncrementEnemyDeck = 400,
            IncrementEnemyHand = 401,
            DecrementEnemyDeck = 402,
            DecrementEnemyHand = 403,
            //server requesting a target of a client
            GetAttackTarget = 500,
            RequestBoardTarget = 501,
            RequestDeckTarget = 502,
            RequestDiscardTarget = 503,
            RequestHandTarget = 504,
            GetChoicesFromList = 505,
            //client responding
            Target = 550,
            SpaceTarget = 551,
            Response = 552,
            DeclineAnotherTarget = 553,
            CancelSearch = 554,
            NoMoreResponse = 555,
            //server notifying if anything else is necessary
            TargetAccepted = 575,
            SpaceTargetAccepted = 576,
            //other effect technicalities
            PlayerSetX = 600,
            EnableDecliningTarget = 601,
            DisableDecliningTarget = 602,
            SetPips = 603,
            SetEnemyPips = 604,
            SetEffectsX = 605,
            OptionalTrigger = 606,
            ChooseEffectOption = 607,
            EffectResolving = 608,
            EffectImpossible = 609,
            //miscellaneous
            DiscardSimples = 700,
            PutBack = 701,
            //effect stuff
            ActivateEffect = 800,
            StackEmpty = 801
        }*/

        /// <summary>
        /// Contains the command that is sent.
        /// </summary>
        public string command;

        public Packet() { }

        public Packet(string command)
        {
            this.command = command;
        }

        public abstract Packet Copy();

        public override string ToString()
        {
            return $"Command: {command}";
        }
    }

}

namespace KompasClient.Networking
{
    public interface IClientOrderPacket
    {
        void Execute(ClientGame clientGame);
    }
}

namespace KompasServer.Networking
{
    public interface IServerOrderPacket
    {
        void Execute(ServerGame serverGame, ServerPlayer player);
    }
}