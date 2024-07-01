using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonLine : MonoBehaviour
{
    public Transform AircraftTransform;
    public float pitchSensitivity = 1.0f; // Sensitivity for the pitch adjustment
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = this.transform.position;
    }

    void Update()
    {

        /*
        // Update rotation based on aircraft's rotation
        this.transform.rotation = new Quaternion(0f, 0f, AircraftTransform.rotation.z, AircraftTransform.rotation.w);

        // Adjust position based on aircraft's pitch
        float pitch = AircraftTransform.rotation.eulerAngles.x;
        if (pitch > 180) pitch -= 360; // Normalize pitch to range -180 to 180

        // Adjust the vertical position based on the pitch
        this.transform.position = initialPosition + Vector3.up * pitch * pitchSensitivity;
        */

        // Adjust position based on aircraft's pitch
        float pitch = AircraftTransform.rotation.eulerAngles.x;
        if (pitch > 180) pitch -= 360; // Normalize pitch to range -180 to 180

        // Adjust the vertical position based on the pitch
        this.transform.position = initialPosition + Vector3.up * pitch * pitchSensitivity;

        // Adjust rotation based on aircraft's roll
        float roll = AircraftTransform.rotation.eulerAngles.z;
        if (roll > 180) roll -= 360; // Normalize roll to range -180 to 180

        // Apply the roll to the horizon line's rotation
        this.transform.rotation = Quaternion.Euler(0f, 0f, roll);


    }
}
