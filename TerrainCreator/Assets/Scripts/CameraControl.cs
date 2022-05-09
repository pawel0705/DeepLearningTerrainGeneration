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

    private float sensitivityX = 5F;

    private float sensitivityY = 5F;

    private float minimumY = -90F;

    private float maximumY = 90F;

    private float rotationY = -90F;

    void Update()
    {
        this.MouseInput();
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
            this.MouseRightClick();
        }
        else if (Input.GetMouseButton(2))
        {
            this.MouseMiddleButtonClicked();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            this.ShowAndUnlockCursor();
        }
        else if (Input.GetMouseButtonUp(2))
        {
            this.ShowAndUnlockCursor();
        }
        else
        {
            this.MouseWheeling();
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
        this.HideAndLockCursor();

        var NewPosition = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
        var pos = this.transform.position;

        if (NewPosition.x > 0.0f)
        {
            pos += this.transform.right * 15f;
        }

        if (NewPosition.x < 0.0f)
        {
            pos -= this.transform.right * 15f;
        }

        if (NewPosition.z > 0.0f)
        {
            pos += this.transform.forward * 15f;
        }

        if (NewPosition.z < 0.0f)
        {
            pos -= this.transform.forward * 15f;
        }

        pos.y = this.transform.position.y;
        this.transform.position = pos;
    }

    void MouseRightClick()
    {
        this.HideAndLockCursor();

        if (axes == RotationAxes.MouseXAndY)
        {
            var rotationX = this.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivityX;

            this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
            this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);

            this.transform.localEulerAngles = new Vector3(-this.rotationY, rotationX, 0);
        }
        else if (axes == RotationAxes.MouseX)
        {
            this.transform.Rotate(0, Input.GetAxis("Mouse X") * this.sensitivityX, 0);
        }
        else
        {
            this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
            this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);

            this.transform.localEulerAngles = new Vector3(-this.rotationY, this.transform.localEulerAngles.y, 0);
        }
    }

    void MouseWheeling()
    {
        var pos = this.transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            pos = pos - this.transform.forward * 115f;
            this.transform.position = pos;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            pos = pos + this.transform.forward * 115f;
            this.transform.position = pos;
        }
    }
}
