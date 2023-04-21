using System.Collections.Generic;
using KompasClient.Effects;
using KompasCore.Cards;
using KompasCore.Effects;

public class ClientSerializableCard : SerializableGameCard
{
	public ClientEffect[] effects;

	public override IEnumerable<Effect> Effects => effects;
}
