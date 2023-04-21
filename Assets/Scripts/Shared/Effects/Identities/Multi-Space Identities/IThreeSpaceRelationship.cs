namespace KompasCore.Effects.Identities
{
	/// <summary>
	/// Describes a relationship between three spaces.
	/// </summary>
	public interface IThreeSpaceRelationship
	{
		/// <summary>
		/// By convention, spaces a and b describe a relationship that space c will be tested to fit.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public bool Evaluate(Space a, Space b, Space c);
	}

	namespace ThreeSpaceRelationships
	{
		public class Between : IThreeSpaceRelationship
		{
			public bool Evaluate(Space a, Space b, Space c)
				=> c.Between(a, b);
		}
	}
}