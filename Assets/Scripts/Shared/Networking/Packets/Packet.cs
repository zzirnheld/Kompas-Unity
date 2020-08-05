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

        //player action: things the player initiates (client to server)
        public const string PlayAction = "Player Play Action";
        public const string AugmentAction = "Player Augment Action";
        public const string MoveAction = "Player Move Action";
        public const string AttackAction = "Player Attack Action";
        public const string EndTurnAction = "Player End Turn Action";
        public const string ActivateEffectAction = "Player Activate Effect Action";

        //effect commands
            //from client to server
        public const string CardTargetChosen = "Card Target Chosen";
        public const string SpaceTargetChosen = "Space Target Chosen";
        public const string XSelectionChosen = "X Value Chosen";
        public const string DeclineAnotherTarget = "Decline Another Target";
        public const string ListChoicesChosen = "List Choices Chosen";
        public const string OptionalTriggerResponse = "Optional Trigger Answered";
        public const string ChooseEffectOption = "Choose Effect Option";

        //gamestate (from server to client)
        public const string SetLeyload = "Set Leyload";

        //card addition/deletion (from server to client)
        public const string DeleteCard = "Delete Card";
        public const string AddCard = "Add Card";
        public const string ChangeEnemyHandCount = "Change Enemy Hand Count";

        //card movement (from server to client)
            //public locations
        public const string PlayCard = "Play Card";
        public const string AttachCard = "Attach Card";
        public const string MoveCard = "Move Card";
        public const string DiscardCard = "Discard Card";
        public const string AnnihilateCard = "Annihilate Card";
            //private locations
        public const string RehandCard = "Rehand Card";
        public const string TopdeckCard = "Topdeck Card";
        public const string ReshuffleCard = "Reshuffle Card";
        public const string BottomdeckCard = "Bottomdeck Card";

        //stats
        public const string UpdateCardNumericStats = "Change Card Numeric Stats";

        //debug commands
        //from client to server
        public const string DebugTopdeck = "DEBUG COMMAND Topdeck";
        public const string DebugDiscard = "DEBUG COMMAND Discard";
        public const string DebugRehand = "DEBUG COMMAND Rehand";
        public const string DebugDraw = "DEBUG COMMAND Draw";
        public const string DebugSetNESW = "DEBUG COMMAND Set NESW";
        public const string DebugSetPips = "DEBUG COMMAND Set Pips";

        /// <summary>
        /// Contains the command that is sent.
        /// </summary>
        public string command;

        public Packet() { }

        public Packet(string command)
        {
            this.command = command;
        }

        /// <summary>
        /// Creates an exact copy of this packet to send.
        /// </summary>
        /// <returns></returns>
        public abstract Packet Copy();
        /// <summary>
        /// Creates a version of this packet that the opposite player will understand.
        /// </summary>
        /// <returns></returns>
        public virtual Packet GetInversion(bool known) => known ? Copy() : null;

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