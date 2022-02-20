using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    private enum RotationAxes 
    { 
        MouseXAndY = 0, 
        MouseX = 1, 
        MouseY = 2 
    }

    private RotationAxes axes = RotationAxes.MouseXAndY;

    private float sensitivityX = 2F;

    private float sensitivityY = 2F;

    private float minimumX = -360F;

    private float maximumX = 360F;

    private float minimumY = -90F;

    private float maximumY = 90F;

    private float rotationY = -90F;

    void Update()
    {
        MouseInput();
    }

    void MouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
        }
        else if (Input.GetMouseButton(1))
        {
            MouseRightClick();
        }
        else if (Input.GetMouseButton(2))
        {
            MouseMiddleButtonClicked();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ShowAndUnlockCursor();
        }
        else if (Input.GetMouseButtonUp(2))
        {
            ShowAndUnlockCursor();
        }
        else
        {
            MouseWheeling();
        }
    }

    void ShowAndUnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void MouseMiddleButtonClicked()
    {
        HideAndLockCursor();
        Vector3 NewPosition = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
        Vector3 pos = transform.position;
        if (NewPosition.x > 0.0f)
        {
            pos += transform.right;
        }
        if (NewPosition.x < 0.0f)
        {
            pos -= transform.right;
        }
        if (NewPosition.z > 0.0f)
        {
            pos += transform.forward;
        }
        if (NewPosition.z < 0.0f)
        {
            pos -= transform.forward;
        }
        pos.y = transform.position.y;
        transform.position = pos;
    }

    void MouseRightClick()
    {
        HideAndLockCursor();
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        }
    }

    void MouseWheeling()
    {
        Vector3 pos = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            pos = pos - transform.forward * 4f;
            transform.position = pos;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            pos = pos + transform.forward * 4f;
            transform.position = pos;
        }
    }
}
