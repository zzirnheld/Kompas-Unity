using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableCommand
{
    protected ServerGame serverGame;

    public virtual void StartResolution()
    {
        Debug.Log("This is a test stack entry. just goes to the next one");
        FinishResolution();
    }

    public virtual void FinishResolution()
    {
        serverGame.ResolveNextStackEntry();
    }

    public virtual void CancelResolution()
    {
        FinishResolution();
    }
}
