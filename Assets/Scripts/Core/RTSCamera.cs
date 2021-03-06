using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class RTSCamera : MonoBehaviour
{
    public float cameraMoveSpeed = 10;
    //public float cameraRotateSpeed = 100;
    public float cameraScrollSpeed = 300;

    public bool cameraFollow = true;
    //public float minXPos = 30;
    //public float maxXPos = 50;

    //public float minYPos = 10;
    //public float maxYPos = 30;

    //public float minZPos = 15;
    //public float maxZPos = 30;

    public enum CameraState
    {
        idle,
        move,
        rotate,
    }
    
    [SerializeField]
    private CameraState cameraState = CameraState.idle;

    private GameObject anchor;

    Tween cameraMove;

    private void Start()
    {
        anchor = new GameObject("CameraAnchor");    
    }

    void LateUpdate()
    {
        //float currentX;
        //float currentY;
        //float currentZ;
        
#if (UNITY_IOS || UNITY_ANDROID)
        //if (Input.touchCount == 2)
        //{
        //    // Store both touches.
        //    Touch touchZero = Input.GetTouch(0);
        //    Touch touchOne = Input.GetTouch(1);

        //    // Find the position in the previous frame of each touch.
        //    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        //    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        //    // Find the magnitude of the vector (the distance) between the touches in each frame.
        //    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        //    // Find the difference in the distances between each frame.
        //    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        //    deltaMagnitudeDiff = Mathf.Clamp(deltaMagnitudeDiff, -1, 1);
        //    // Otherwise change the field of view based on the change in distance between the touches.
        //    camera.fieldOfView += deltaMagnitudeDiff * cameraScrollSpeed / 50 * Time.deltaTime;
        //    camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, 10, 50);
        //    //currentY += deltaMagnitudeDiff * cameraScrollSpeed / 100 * Time.deltaTime;
        //    //currentZ -= deltaMagnitudeDiff * cameraScrollSpeed / 100 * Time.deltaTime;
        //}
        //else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        //{
        //    currentX -= Input.GetTouch(0).deltaPosition.x / 80.0f * cameraMoveSpeed * Time.deltaTime;
        //    currentZ -= Input.GetTouch(0).deltaPosition.y / 80.0f * cameraMoveSpeed * Time.deltaTime;
        //}
#elif (UNITY_STANDALONE || UNITY_EDITOR)

        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (!EventSystem.current.IsPointerOverGameObject() && cameraState == CameraState.idle)
        {
            var hor = Vector3.Cross(Vector3.up, transform.right);

            if (Input.mousePosition.x <= 20)
            {
                transform.Translate(-transform.right * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.x >= (Screen.width - 20))
            {
                transform.Translate(transform.right * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.y <= 20)
            {
                transform.Translate(hor * cameraMoveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.mousePosition.y >= (Screen.height - 20))
            {
                transform.Translate(-hor * cameraMoveSpeed * Time.deltaTime, Space.World);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateCamera(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateCamera(false);
            }

            transform.Translate(transform.forward * mouseWheel * cameraScrollSpeed * Time.deltaTime, Space.World);
        }

        
#endif
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            //currentX = transform.position.x;
            //currentY = transform.position.y;
            //currentZ = transform.position.z;
            
            //currentX = transform.localPosition.x;
            //currentY = transform.localPosition.y;
            //currentZ = transform.localPosition.z;
            
            //currentX = Mathf.Clamp(currentX, minXPos, maxXPos);
            //currentY = Mathf.Clamp(currentY, minYPos, maxYPos);
            //currentZ = Mathf.Clamp(currentZ, minZPos - currentY, maxZPos - currentY);

            //transform.position = new Vector3(currentX, currentY, currentZ);
        }

        

    }

    //public void FollowTarget(Transform target) 
    //{
    //    if (cameraFollow && cameraState != CameraState.rotate)
    //    {
    //        var position = target.position - transform.forward * (transform.position.y / Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.x));
    //        if (cameraMove != null)
    //            cameraMove.Kill();
    //        cameraMove = transform.DOMove(position, 0.5f).OnPlay(() => {
    //            cameraState = CameraState.move;
    //        }).OnComplete(() => {
    //            cameraState = CameraState.idle;
    //        });
    //    }
    //}

    public void FollowTarget(Vector3 targetPosition)
    {
        if (cameraFollow && cameraState != CameraState.rotate)
        {
            var position = targetPosition - transform.forward * (transform.position.y / Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.x));
            if (cameraMove != null)
                cameraMove.Kill();
            cameraMove = transform.DOMove(position, 0.5f).OnPlay(() => {
                cameraState = CameraState.move;
            }).OnComplete(() => {
                cameraState = CameraState.idle;
            });
        }
    }

    public void RotateCamera(bool left)
    {
        if(cameraState == CameraState.idle)
        {
            anchor.transform.position = transform.position.y * 2 * transform.forward + transform.position;
            transform.SetParent(anchor.transform);
            if (left)
            {
                anchor.transform.DORotate(new Vector3(0, anchor.transform.rotation.eulerAngles.y + 90, 0), 0.5f).OnPlay(() => {
                    cameraState = CameraState.rotate;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    transform.SetParent(null);
                });
            }
            else
            {
                anchor.transform.DORotate(new Vector3(0, anchor.transform.eulerAngles.y - 90, 0), 0.5f).OnPlay(() => {
                    cameraState = CameraState.rotate;
                }).OnComplete(() => {
                    cameraState = CameraState.idle;
                    transform.SetParent(null);
                });
            }
        }
    }
}
