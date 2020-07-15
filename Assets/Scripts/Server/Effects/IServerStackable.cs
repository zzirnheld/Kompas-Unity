using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServerStackable : IStackable
{
    void StartResolution(ActivationContext context);

    ServerPlayer ServerController { get; }
}
