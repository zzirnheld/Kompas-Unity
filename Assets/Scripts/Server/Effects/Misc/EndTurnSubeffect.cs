namespace KompasServer.Effects
{
    public class EndTurnSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerGame.SwitchTurn();
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}