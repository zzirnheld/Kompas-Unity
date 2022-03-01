﻿using KompasClient.GameCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClientCameraController : MonoBehaviour
{
    public static ClientCameraController Main { get; private set; }

    public const float ZoomFactor = 1f;
    public const float PanFactorBase = 0.4f;
    public const float RotationFactorBase = 1f;
    public const float MinCameraHeight = 2f;
    public const float MaxCameraHeight = 30f;
    public const float MaxCameraPan = 12f;

    public static float ZoomThreshold = 14f;

    public float PanFactor => Mathf.Log10(transform.position.y) * PanFactorBase;
    public float RotationAngle => Mathf.Log10(transform.position.y) * RotationFactorBase;
    public bool Zoomed => transform.position.y <= ZoomThreshold;

    public Vector3 Down => PanFactor * Vector3.down;
    public Vector3 Up => PanFactor * Vector3.up;
    public Vector3 Left => PanFactor * Vector3.left;
    public Vector3 Right => PanFactor * Vector3.right;

    public ClientGame clientGame;

    private void Awake()
    {
        Main = this;
    }

    public void FixedUpdate()
    {
        if (transform.position.y > MinCameraHeight || Input.mouseScrollDelta.y < 0)
        {
            var tempHeight = transform.position.y;
            if (clientGame.canZoom && !EventSystem.current.IsPointerOverGameObject())
                transform.Translate(ZoomFactor * Input.mouseScrollDelta.y * Vector3.forward);

            //if just crossed the threshold for showing cards as zoomed or no, update cards accordingly
            if (tempHeight > ZoomThreshold && transform.position.y <= ZoomThreshold)
                clientGame.ShowCardsByZoom(true);
            else if (tempHeight <= ZoomThreshold && transform.position.y > ZoomThreshold)
                clientGame.ShowCardsByZoom(false);
        }

        if (Input.GetKey(KeyCode.W)) transform.Translate(Up);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Down);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Left);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Right);

        if (Input.GetKey(KeyCode.Q)) transform.Rotate(Vector3.back, RotationAngle);
        if (Input.GetKey(KeyCode.E)) transform.Rotate(Vector3.forward, RotationAngle);

        if (Input.GetKeyUp(KeyCode.Space)) transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }
}
