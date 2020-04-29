using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackable
{
    void StartResolution();

    Player Controller { get; }
    Card Source { get; }
}
