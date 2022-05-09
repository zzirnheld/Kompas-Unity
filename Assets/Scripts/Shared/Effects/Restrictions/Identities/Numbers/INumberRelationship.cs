namespace KompasCore.Effects.Relationships
{
    public interface INumberRelationship
    {
        public bool Compare(int a, int b);
    }

    namespace NumberRelationships
    {
        public class GreaterThan : INumberRelationship
        {
            public bool Compare(int a, int b) => a > b;
        }

        public class Equal : INumberRelationship
        {
            public bool Compare(int a, int b) => a == b;
        }
    }
}