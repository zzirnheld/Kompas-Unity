namespace KompasServer.Effects
{
    public class SetXByTargetValueSubeffect : SetXSubeffect
    {
        public const string Cost = "Cost";
        public const string N = "N";
        public const string E = "E";
        public const string S = "S";
        public const string W = "W";
        public const string C = "C";
        public const string DistanceToTarget = "Distance to Target";

        public string whatToCount;

        public override int BaseCount
        {
            get
            {
                switch (whatToCount)
                {
                    case Cost: return Target.Cost;
                    case N: return Target.N;
                    case E: return Target.E;
                    case S: return Target.S;
                    case W: return Target.W;
                    case C: return Target.C;
                    case DistanceToTarget:
                        return Source.DistanceTo(Target);
                    default:
                        throw new System.ArgumentException($"Invalid 'what to count' string {whatToCount} in x by gamestate value subeffect");
                }
            }
        }
    }
}