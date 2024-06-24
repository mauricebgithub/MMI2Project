using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonLine : MonoBehaviour
{
    public Transform AircraftTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = new Quaternion(0f, 0f, AircraftTransform.rotation.z, AircraftTransform.rotation.w);
    }
}
