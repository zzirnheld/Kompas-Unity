using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServerStackable : IStackable
{
    void StartResolution(int startIndex = 0);

    ServerPlayer ServerController { get; }
}
