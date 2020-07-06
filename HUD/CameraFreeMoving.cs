using UnityEngine;

[AddComponentMenu("Camera/Simple Smooth Mouse Look ")]
public class CameraFreeMoving : MonoBehaviour
{
    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    float flySpeed = 0.5f;
    
    public bool MovementEnabled;
    public bool LookEnabled;
    bool shift;
    bool ctrl;
    float accelerationAmount = 3;
    float accelerationRatio = 1;
    float slowDownRatio = 0.5f;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    public GameObject characterBody;

    void Start()
    {
        
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

 
    }

    void Update()
    {
        if (MovementEnabled)
        {
            moveCamera();
        }

        if (LookEnabled)
        {
            LookCamera();
        }


    }
    void moveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            shift = true;
            flySpeed *= accelerationRatio;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            shift = false;
            flySpeed /= accelerationRatio;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            ctrl = true;
            flySpeed *= slowDownRatio;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            ctrl = false;
            flySpeed /= slowDownRatio;
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.
            transform.Translate(flySpeed * -gameObject.transform.forward * Input.GetAxis("Vertical"));
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Translate( -gameObject.transform.right * flySpeed * Input.GetAxis("Horizontal"));
        } 
        if (Input.GetKey(KeyCode.E))
        {
            //transform.up
            transform.Translate(transform.up * flySpeed * 0.5f);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(-gameObject.transform.up * flySpeed * 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.F12))
            switchCamera();
        
    }

    void LookCamera()
    {
        // Ensure the cursor is always locked when set
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody)
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else
        {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }
    }
    void switchCamera()
    {
        //if (!isEnabled) //means it is currently disabled. code will enable the flycam. you can NOT use 'enabled' as boolean's name.
        //{
        //    transform.position = defaultCam.transform.position; //moves the flycam to the defaultcam's position
        //    defaultCam.camera.active = false;
        //    this.camera.active = true;
        //    isEnabled = true;
        //}
        //else if (isEnabled) //if it is not disabled, it must be enabled. the function will disable the freefly camera this time.
        //{
        //    this.camera.active = false;
        //    defaultCam.camera.active = true;
        //    isEnabled = false;
        //}
    }
}