using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public Vector3 playerFollowOffset;
    public Vector3 islandFollowOffset;

    public Vector2 minMaxCameraDistance;
    public float cameraDistance;

    [SerializeField] Camera playerCamera;
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    //private float targetFOV;

    private Camera cam;

    private float lastTouchDistance;

    public enum CameraMode
    {
        FollowPlayer,
        RotateAroundIsland,
        Fishing,
        Shopping,
        Fixed
    }

    public CameraMode mode;

    public void IsolatePlayer()
    {
        cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
        playerCamera.enabled = true;
    }

    public void StopIsolatingPlayer()
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer("Player");
        playerCamera.enabled = false;
    }

    private void Start()
    {
        mode = CameraMode.RotateAroundIsland;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        //if (Input.touchCount > 1)
        //{
        //    Touch first = Input.GetTouch(0);
        //    Touch second = Input.GetTouch(1);

        //    float touchDistance = (first.position - second.position).magnitude;

        //    cameraDistance = Mathf.Clamp(cameraDistance - (touchDistance-lastTouchDistance), minMaxCameraDistance.x, minMaxCameraDistance.y);

        //    lastTouchDistance = (first.position - second.position).magnitude;
        //}

        //cameraDistance = Mathf.Clamp(cameraDistance - Input.mouseScrollDelta.y, minMaxCameraDistance.x, minMaxCameraDistance.y);
        //targetFOV = 60f;

        switch (mode)
        {
            case CameraMode.FollowPlayer:
                {
                    targetRotation = Quaternion.LookRotation(Player.Instance.transform.transform.position - transform.position, Vector3.up);
                    targetPosition = Player.Instance.transform.position + playerFollowOffset * cameraDistance;
                    //targetFOV = 90f;
                }
                break;
            case CameraMode.RotateAroundIsland:
                {
                    GameData.IslandCenter.Rotate(0f, Time.unscaledDeltaTime * 7f, 0f);
                    targetRotation = Quaternion.LookRotation(GameData.IslandCenter.position - transform.position, Vector3.up);
                    targetPosition = GameData.IslandCenterPositionHolder.position;
                }
                break;
            case CameraMode.Fishing:
                {
                    targetRotation = GameData.FishingCameraSpot.rotation;
                    targetPosition = GameData.FishingCameraSpot.position;
                }
                break;
            case CameraMode.Shopping:
                {
                    targetRotation = GameData.activeShop.cameraPoint.rotation;
                    targetPosition = GameData.activeShop.cameraPoint.position;
                }
                break;
            case CameraMode.Fixed:
                { 
                    //Do not update posrot
                }
                break;
        }

        //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.unscaledDeltaTime * 2f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * 2f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * 2f);
    }
}
