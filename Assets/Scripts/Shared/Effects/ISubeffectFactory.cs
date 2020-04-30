public interface ISubeffectFactory
{
    ServerSubeffect FromJson(SubeffectType subeffectType, string json, Effect parent, int subeffIndex);
}
