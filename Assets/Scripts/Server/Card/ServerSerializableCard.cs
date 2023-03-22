using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Effects;

public class ServerSerializableCard : SerializableCard
{
    public ServerEffect[] effects;

    public override IEnumerable<Effect> Effects => effects;
}
