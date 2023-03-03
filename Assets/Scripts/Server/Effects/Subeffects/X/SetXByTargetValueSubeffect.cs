namespace KompasServer.Effects.Subeffect
{
    public class SetXByTargetValueSubeffect : SetXSubeffect
    {
        //TODO refactor away these # based ones to SetXByTargetCardValue
        public const string Cost = "Cost";
        public const string N = "N";
        public const string E = "E";
        public const string S = "S";
        public const string W = "W";
        public const string C = "C";

        public const string DistanceToTarget = "Distance to Target";
        public const string DistanceBetweenTargetAndCoords = "Distance Between Target and Target Space";

        public string whatToCount;

        public override int BaseCount
        {
            get
            {
                return whatToCount switch
                {
                    Cost => CardTarget.Cost,
                    N => CardTarget.N,
                    E => CardTarget.E,
                    S => CardTarget.S,
                    W => CardTarget.W,
                    C => CardTarget.C,
                    DistanceToTarget => Source.DistanceTo(CardTarget),
                    DistanceBetweenTargetAndCoords => CardTarget.DistanceTo(SpaceTarget),
                    _ => throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect"),
                };
            }
        }
    }
}