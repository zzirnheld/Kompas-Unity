using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServerStackable : IStackable
{
    ServerPlayer ServerController { get; }
}
