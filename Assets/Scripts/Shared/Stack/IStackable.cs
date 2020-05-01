using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStackable
{
    Player Controller { get; }
    Card Source { get; }
}
