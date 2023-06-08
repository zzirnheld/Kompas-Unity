using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Effects;

public class ServerSerializableCard : SerializableGameCard
{
	public ServerEffect[] effects;

	public override IEnumerable<Effect> Effects => effects;
}
