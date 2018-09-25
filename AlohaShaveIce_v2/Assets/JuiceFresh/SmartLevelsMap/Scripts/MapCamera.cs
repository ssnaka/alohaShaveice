using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

public class MapCamera : MonoBehaviour
{
    public event Action OnCameraMove;

    private Vector2 _prevPosition;

    public Camera Camera;
    public Bounds Bounds;
    Vector2 firstV;
    Vector2 deltaV;
    private float currentTime;
    private float speed;
    bool touched;

    GameObject avatarObject;

    const float waypointBottomBuffer = 0.1f;

    bool enableCameraMove = true;
    float cameraMovingBackSpeed = 30.0f;

    Vector3 cameraMovingBackPosition = new Vector3(0.0f, 4.5f, -10.0f);

    public void Awake ()
    {
        currentTime = 0;
        speed = 0;
    }

    public void OnDrawGizmos ()
    {
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }

    public void Update ()
    {

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
			HandleTouchInput();
#else
        HandleMouseInput();
#endif
    }

    void LateUpdate ()
    {
        SetPosition(transform.position);
    }

    private void HandleTouchInput ()
    {
        int touchId = -1;
        #if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		touchId = 0;
        #endif

        if (!enableCameraMove)
        {
            return;
        }
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touchId))
                {
                    return;
                }

                touched = true;
                deltaV = Vector2.zero;
                _prevPosition = touch.position;
                firstV = _prevPosition;
                currentTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 curPosition = touch.position;
                MoveCamera(_prevPosition, curPosition);
                deltaV = _prevPosition - curPosition;
                _prevPosition = curPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touched = false;
            }
        }
        else if (!touched)
        {
            if (!enableCameraMove)
            {
                return;
            }

            if (MoveCameraBackToWaypoint())
            {
                deltaV = Vector2.zero;
                return;
            }

            deltaV -= deltaV * Time.deltaTime * 10;
            transform.Translate(deltaV.x / 30, deltaV.y / 30, 0);
        }

    }

    private void HandleMouseInput ()
    {
        int touchId = -1;
        #if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		touchId = 0;
        #endif

        if (EventSystem.current.IsPointerOverGameObject(touchId))
        {
            return;
        }

        if (!enableCameraMove)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            deltaV = Vector2.zero;
            _prevPosition = Input.mousePosition;
            firstV = _prevPosition;
            currentTime = Time.time;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 curMousePosition = Input.mousePosition;
            MoveCamera(_prevPosition, curMousePosition);
            deltaV = _prevPosition - curMousePosition;

            _prevPosition = curMousePosition;
            speed = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            speed = (Time.time - currentTime);
            Vector3 diffV = (transform.position - (Vector3)deltaV);
            Vector3 destination = (transform.position - diffV / 20);
        }
        else
        {
            if (!enableCameraMove)
            {
                return;
            }

            if (MoveCameraBackToWaypoint())
            {
                deltaV = Vector2.zero;
                return;
            }

            deltaV -= deltaV * Time.deltaTime * 10;
            transform.Translate(deltaV.x / 30, deltaV.y / 30, 0);
        }

    }

    private void MoveCamera (Vector2 prevPosition, Vector2 curPosition)
    {
        int touchId = -1;
        #if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		touchId = 0;
        #endif
        if (EventSystem.current.IsPointerOverGameObject(touchId))
            return;

        Vector2 newPosition = transform.localPosition + (Camera.ScreenToWorldPoint(prevPosition) - Camera.ScreenToWorldPoint(curPosition));
        SetPosition(newPosition);
    }

    bool MoveCameraBackToWaypoint ()
    {
        Vector3 viewPortPoint = Camera.WorldToViewportPoint(LevelsMap._instance.WaypointsMover.transform.position);
        if (viewPortPoint.y <= waypointBottomBuffer)
        {
            StartCoroutine(MoveCameraBackToWaypointRoutine(transform.position, cameraMovingBackPosition, cameraMovingBackSpeed));
            return true;
        }

        return false;
    }

    IEnumerator MoveCameraBackToWaypointRoutine (Vector3 _fromPosition, Vector3 _toPosition, float _speed)
    {
        enableCameraMove = false;
        Vector3 newToPosition = _toPosition - new Vector3(0.0f, 3.0f, 0.0f);

//        if (LevelManager.Instance.currentLevel <= 3)
//        {
//            newToPosition = _toPosition - new Vector3(0.0f, 1.0f, 0.0f);
//        }

        float step = (_speed / (_fromPosition - newToPosition).magnitude) * Time.deltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            Vector3 position = Vector3.Lerp(_fromPosition, newToPosition, t);
            SetPosition(position);
            yield return new WaitForEndOfFrame();
        }

        if (LevelManager.Instance.currentLevel <= 3)
        {
            enableCameraMove = true;
            SetPosition(newToPosition);
            yield break;
        }

        step = (_speed / 3.0f / (newToPosition - _toPosition).magnitude) * Time.deltaTime;
        t = 0;
        while (t <= 1.0f)
        {
            t += step;
            Vector3 position = Vector3.Lerp(newToPosition, _toPosition, t);
            SetPosition(position);
            yield return new WaitForEndOfFrame();
        }


        enableCameraMove = true;
        SetPosition(_toPosition);
    }

    public void SetPosition (Vector2 position)
    {
        Vector2 validatedPosition = ApplyBounds(position);
        transform.position = new Vector3(validatedPosition.x, validatedPosition.y, transform.position.z);

        if (OnCameraMove != null)
        {
            OnCameraMove();
        }
    }

    private Vector2 ApplyBounds (Vector2 position)
    {
        float cameraHeight = Camera.orthographicSize * 2f;
        float cameraWidth = (Screen.width * 1f / Screen.height) * cameraHeight;
        position.x = Mathf.Max(position.x, Bounds.min.x + cameraWidth / 2f);
        position.y = Mathf.Max(position.y, Bounds.min.y + cameraHeight / 2f);
        position.x = Mathf.Min(position.x, Bounds.max.x - cameraWidth / 2f);
        position.y = Mathf.Min(position.y, Bounds.max.y - cameraHeight / 2f);
//		position.y = Mathf.Max(position.y, 0.0f);
        return position;
    }

    public void SetCameraBounds (List<Sprite> _bgSpriteList)
    {
        float totalHeight = 0.0f;
        for (int i = 0; i < _bgSpriteList.Count; i++)
        {
            totalHeight += _bgSpriteList[i].bounds.size.y;
        }

        Bounds = new Bounds(
            new Vector3(Bounds.center.x, totalHeight / 2.0f - _bgSpriteList[0].bounds.size.y / 2.0f, Bounds.center.z),
            new Vector3(Bounds.size.x, totalHeight, Bounds.size.z));
    }

}
