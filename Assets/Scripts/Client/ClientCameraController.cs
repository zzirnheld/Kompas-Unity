using KompasClient.GameCore;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClientCameraController : MonoBehaviour
{
    public static ClientCameraController Main { get; private set; }
    public static bool MainZoomed => Main == null ? false : Main.Zoomed;

    public const float ZoomFactor = 3f;
    public const float PanFactorBase = 4f;
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

    public GameObject selectedCardGameObject;

    public Camera mainCamera;

    private void Awake()
    {
        Main = this;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0f)
        {
            //Shift-scroll means rotate
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                transform.Rotate(Vector3.forward * Input.mouseScrollDelta.y, RotationAngle);
            }
            //Normal scroll means zoom
            else if (transform.position.y > MinCameraHeight || Input.mouseScrollDelta.y < 0)
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
        }


        if (Input.GetKey(KeyCode.W)) transform.Translate(Up * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Down * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Left * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Right * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q)) transform.Rotate(Vector3.back * Time.deltaTime, RotationAngle);
        if (Input.GetKey(KeyCode.E)) transform.Rotate(Vector3.forward * Time.deltaTime, RotationAngle);

        if (Input.GetKeyUp(KeyCode.Space)) transform.eulerAngles = new Vector3(90f, 0f, 0f);
    }
}
