namespace KompasServer.Effects
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
                    Cost => Target.Cost,
                    N => Target.N,
                    E => Target.E,
                    S => Target.S,
                    W => Target.W,
                    C => Target.C,
                    DistanceToTarget => Source.DistanceTo(Target),
                    DistanceBetweenTargetAndCoords => Target.DistanceTo(Space),
                    _ => throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect"),
                };
            }
        }
    }
}