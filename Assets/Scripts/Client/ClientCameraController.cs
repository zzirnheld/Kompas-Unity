using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientCameraController : MonoBehaviour
{
    public const float ZoomFactor = 1f;
    public const float PanFactorBase = 0.4f;
    public const float RotationFactorBase = 1f;
    public const float MinCameraHeight = 2f;
    public const float MaxCameraHeight = 30f;
    public const float MaxCameraPan = 12f;

    public float PanFactor => Mathf.Log10(transform.position.y) * PanFactorBase;
    public float RotationAngle => Mathf.Log10(transform.position.y) * RotationFactorBase;

    public Vector3 Down     => PanFactor * Vector3.down;
    public Vector3 Up       => PanFactor * Vector3.up;
    public Vector3 Left     => PanFactor * Vector3.left;
    public Vector3 Right    => PanFactor * Vector3.right;

    public void FixedUpdate()
    {
        if(transform.position.y > MinCameraHeight || Input.mouseScrollDelta.y < 0) 
            transform.Translate(ZoomFactor * Input.mouseScrollDelta.y * Vector3.forward);

        if (Input.GetKey(KeyCode.W)) transform.Translate(Up);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Down);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Left);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Right);

        if (Input.GetKey(KeyCode.Q)) transform.Rotate(Vector3.back, RotationAngle);
        if (Input.GetKey(KeyCode.E)) transform.Rotate(Vector3.forward, RotationAngle);

        if (Input.GetKeyUp(KeyCode.Space)) transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }
}
