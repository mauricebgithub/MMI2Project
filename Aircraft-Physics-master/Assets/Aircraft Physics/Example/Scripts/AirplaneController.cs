using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour
{
    [SerializeField]
    List<AeroSurface> controlSurfaces = null;
    [SerializeField]
    List<WheelCollider> wheels = null;
    [SerializeField]
    float rollControlSensitivity = 0.2f;
    [SerializeField]
    float pitchControlSensitivity = 0.2f;
    [SerializeField]
    float yawControlSensitivity = 0.2f;
    [SerializeField]
    float mouseSensetivity = 1.0f;
    [SerializeField]
    float throttleSpeed = 0.1f;

    public enum ControlMode
    {
        MouseControl,
        KeyboardControl,
        JoystickControl
    }

    [SerializeField]
    private ControlMode controlMode = ControlMode.KeyboardControl;

    [Range(-1, 1)]
    public float Pitch;
    [Range(-1, 1)]
    public float Yaw;
    [Range(-1, 1)]
    public float Roll;
    [Range(0, 1)]
    public float Flap;
    [SerializeField]
    Text displayText = null;
    [SerializeField]
    Text contolHelpText = null;
    [SerializeField]
    RectTransform arrowUI = null;

    float thrustPercent;
    float brakesTorque;

    AircraftPhysics aircraftPhysics;
    Rigidbody rb;

    //Added variables
    public bool visibleControls = true;
    GameObject controlsDisplay;

    private ControlMode lastControlMode;


    private void Start()
    {
        aircraftPhysics = GetComponent<AircraftPhysics>();
        rb = GetComponent<Rigidbody>();
        controlsDisplay = GameObject.Find("Controls");

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        UpdateControlHelpText();
        
        
    }

    private void Update()
    {

        // Check if controlMode has changed
        if (controlMode != lastControlMode)
        {
            UpdateControlHelpText();
            lastControlMode = controlMode;
        }


        switch (controlMode)
        {
            case ControlMode.MouseControl:
                

                // Use mouse movement for pitch and roll
                // Pitch = -Input.GetAxis("Mouse Y") * mouseSensetivity;
                // Roll = Input.GetAxis("Mouse X") * mouseSensetivity;
                Vector3 mousePos = Input.mousePosition;
                // Normalize and map mouse position to pitch and roll
                mousePos.x = ((mousePos.x * mouseSensetivity) / Screen.width) * 2 - 1; // Normalize to [-1, 1]
                mousePos.y = ((mousePos.y * mouseSensetivity) / Screen.height) * 2 - 1; // Normalize to [-1, 1]

                Roll = Mathf.Clamp(mousePos.x, -1, 1);
                Pitch = Mathf.Clamp(mousePos.y, -1, 1); // Invert Y for natural control
                // Space to toggle thrust                
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    thrustPercent = thrustPercent > 0 ? 0 : 1f;
                }
                // Press B To Brake
                if (Input.GetKeyDown(KeyCode.B))
                {
                    brakesTorque = brakesTorque > 0 ? 0 : 100f;
                }
                break;
            case ControlMode.KeyboardControl:
                // Use keyboard arrows for pitch and roll
                Pitch = Input.GetAxis("Vertical");
                Roll = Input.GetAxis("Horizontal");

                // Space to toggle thrust
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    thrustPercent = thrustPercent > 0 ? 0 : 1f;
                }

                // Press B To Brake
                if (Input.GetKeyDown(KeyCode.B))
                {
                    brakesTorque = brakesTorque > 0 ? 0 : 100f;
                }
                break;
            case ControlMode.JoystickControl:
                Pitch = Input.GetAxis("Vertical");
                Roll = Input.GetAxis("Horizontal");
                Yaw = Input.GetAxis("Yaw");

                float throttleInput = Input.GetAxis("ThrottleInc"); // RT
                float reverseThrottleInput = Input.GetAxis("ThrottleDec"); // LT
                float throttleTest = Input.GetAxis("ThrottleTest");

                //thrustPercent = thrustSlider.value;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    thrustPercent = thrustPercent > 0 ? 0 : 1f;
                    /*thrustPercent = thrustPercent + 0.001f;
                    if(thrustPercent > 1f)
                    {
                        thrustPercent = 1f;
                    }*/
                }

                if (Input.GetKeyDown(KeyCode.JoystickButton3))
                {
                    Flap = Flap > 0 ? 0 : 0.3f;
                }
                if (Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    brakesTorque = brakesTorque > 0 ? 0 : 100f;
                }

                // Steuere die Throttle basierend auf den Trigger-Eingaben
                //float throttleChange = throttleInput - reverseThrottleInput; // Differenz zwischen RT und LT
                float throttleChange = throttleTest;
                thrustPercent += throttleChange * throttleSpeed * Time.deltaTime; // Anpassung der Throttle 

                // Begrenze thrustPercent auf den Bereich zwischen 0 und 1
                thrustPercent = Mathf.Clamp01(thrustPercent);



                //Debugging Statements:
                //Debug.Log("ThrottleInc input: " + throttleInput);
                //Debug.Log("ThrottleDec input: " + reverseThrottleInput);
                //Debug.Log("ThrottleTest input: " + throttleTest);
            break;
        }

        // Optional: control flaps and brakes as before
        if (Input.GetKeyDown(KeyCode.F))
        {
            Flap = Flap > 0 ? 0 : 0.3f;
        }



        // Display controls visibility
        if (visibleControls)
        {
            if (!controlsDisplay.activeSelf)
            {
                controlsDisplay.SetActive(true);
            }
            displayText.text = "V: " + ((int)rb.velocity.magnitude).ToString("D3") + " m/s\n";
            displayText.text += "A: " + ((int)transform.position.y).ToString("D4") + " m\n";
            displayText.text += "T: " + (int)(thrustPercent * 100) + "%\n";
            displayText.text += brakesTorque > 0 ? "B: ON" : "B: OFF";
        }
        else
        {
            controlsDisplay.SetActive(false);
        }
        updateGizmo(Pitch, Roll);
        
    }


    private void FixedUpdate()
    {
        SetControlSurfecesAngles(Pitch, Roll, Yaw, Flap);
        aircraftPhysics.SetThrustPercent(thrustPercent);
        foreach (var wheel in wheels)
        {
            wheel.brakeTorque = brakesTorque;
            wheel.motorTorque = 0.01f; // Small torque to wake up wheel collider
        }
    }

    private void updateGizmo(float pitch, float roll)
    {
        // float CENTER_X = 600.0f;
        // float CENTER_Y = -400.0f;
        float RANGE = 90.0f;
        float RADIUS = 100.0f;
        
        float gizmo_y = pitch * RANGE;
        float gizmo_x = roll * RANGE;
        
        Vector2 gizmoPosition = new Vector2(gizmo_x, gizmo_y);
        Vector2 clampedPosition = Vector2.ClampMagnitude(gizmoPosition, RADIUS);
        
        arrowUI.anchoredPosition  = clampedPosition; 
    }

    public void SetControlSurfecesAngles(float pitch, float roll, float yaw, float flap)
    {
        foreach (var surface in controlSurfaces)
        {
            if (surface == null || !surface.IsControlSurface) continue;
            switch (surface.InputType)
            {
                case ControlInputType.Pitch:
                    surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Roll:
                    surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Yaw:
                    surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer);
                    break;
                case ControlInputType.Flap:
                    surface.SetFlapAngle(Flap * surface.InputMultiplyer);
                    break;
            }
        }
    }

    private void UpdateControlHelpText()
    {
        switch (controlMode)
        {
            case ControlMode.MouseControl:
                contolHelpText.text = "CONTROLS:\n PITCH - Touchpad Vertical position\n ROLL - Touchpad Horizontal position\n FLAPS TOGGLE - F\n BRAKES TOGGLE - B\n THROTTLE TOGGLE - Space\n";
                break;
            case ControlMode.KeyboardControl:
                contolHelpText.text = "CONTROLS:\n PITCH - W/S\n ROLL - A/D\n FLAPS TOGGLE - F\n BRAKES TOGGLE - B\n THROTTLE TOGGLE - Space \n";
                break;
            case ControlMode.JoystickControl:
                contolHelpText.text = "\nCONTROLS:\n PITCH - Left Stick Vertical\n ROLL - Left Stick Horizontal\n YAW - RB/LB\n FLAPS TOGGLE - ControllerY\n BRAKES TOGGLE - B\n THROTTLE TOGGLE - RT/LT\n ";
                break;

        }
    }



    public float getVelocity()
    {
        return rb.velocity.magnitude;
    }

    public float getAltitude()
    {
        return transform.position.y;
    }


}
