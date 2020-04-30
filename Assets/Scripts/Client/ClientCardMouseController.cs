using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCardMouseController : CardMouseController
{
    //constants
    //minimum and maximum distances to the board, discard, and deck objects for dragging
    public const float minBoardLocalX = -0.45f;
    public const float maxBoardLocalX = 0.45f;
    public const float minBoardLocalY = -0.45f;
    public const float maxBoardLocalY = 0.45f;
    public const float minDiscardX = 4.5f;
    public const float maxDiscardX = 5.5f;
    public const float minDiscardZ = -3.5f;
    public const float maxDiscardZ = -2.5f;
    public const float minDeckX = 2.5f;
    public const float maxDeckX = 3.5f;
    public const float minDeckZ = -5.5f;
    public const float maxDeckZ = -4.5f;

    public ClientGame ClientGame;
    public override Game Game => ClientGame;


    //helper methods
    public bool WithinIgnoreY(Vector3 position, float minX, float maxX, float minZ, float maxZ)
    {
        return position.x > minX && position.x < maxX && position.z > minZ && position.z < maxZ;
    }
    public bool WithinIgnoreZ(Vector3 position, float minX, float maxX, float minY, float maxY)
    {
        return position.x > minX && position.x < maxX && position.y > minY && position.y < maxY;
    }

    public override void OnMouseUp()
    {
        base.OnMouseUp();

        //don't allow dragging cards if we're awaiting a target
        if (Card.game.targetMode != Game.TargetMode.Free) return;

        //to be able to use local coordinates to see if you're on the board, set parent to game board
        var boardLocalPosition = Game.boardObject.transform.InverseTransformPoint(transform.position);

        //then, check if it's on the board, accodring to the local coordinates of the game board)
        if (WithinIgnoreZ(boardLocalPosition, minBoardLocalX, maxBoardLocalX, minBoardLocalY, maxBoardLocalY))
        {
            int x = PosToGridIndex(boardLocalPosition.x);
            int y = PosToGridIndex(boardLocalPosition.y);

            //if the card is being moved on the field, that means it's just being moved
            if (Card.Location == CardLocation.Field)
            {
                CharacterCard charThere = Game.boardCtrl.GetCharAt(x, y);
                Debug.Log($"Trying to move/attack to {x}, {y}. The controller index, if any, is {charThere?.ControllerIndex}");
                //then check if it's an attack or not
                if (charThere != null && charThere.Controller != Card.Controller)
                    ClientGame.clientNotifier.RequestAttack(Card, x, y);
                else
                    ClientGame.clientNotifier.RequestMove(Card, x, y);
            }
            //otherwise, it is being played from somewhere like the hand or discard
            else ClientGame.clientNotifier.RequestPlay(Card, x, y);
        }
        //if it's not on the board, maybe it's on top of the discard
        else if (WithinIgnoreY(transform.position, minDiscardX, maxDiscardX, minDiscardZ, maxDiscardZ))
        {
            //in that case, discard it //TODO do this by raycasting along another layer to see if you hit deck/discard
            ClientGame.clientNotifier.RequestDiscard(Card);
        }
        //maybe it's on top of the deck
        else if (WithinIgnoreY(transform.position, minDeckX, maxDeckX, minDeckZ, maxDeckZ))
        {
            //in that case, topdeck it
            ClientGame.clientNotifier.RequestTopdeck(Card);
        }
        //if it's not in any of those, probably should go back in the hand.
        else
        {
            ClientGame.clientNotifier.RequestRehand(Card);
        }
    }
}
