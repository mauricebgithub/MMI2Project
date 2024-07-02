using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public int nRings = 75;
    public static int ringsHit = 0;

    private GameObject landingstrip;

    [SerializeField]
    private Material seeTrough;

    void Start()
    {
        landingstrip = GameObject.FindGameObjectWithTag("Landing");
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
    public void OnTriggerEnter(Collider other)
    {
        ringsHit++;
        Debug.Log("Obstacle hit! Rings:" + ringsHit.ToString() + "/" + nRings.ToString());
//        landingstrip.GetComponent<EvaluateFlight>().obstacleWasHit();
        this.GetComponent<MeshRenderer>().material= seeTrough;
        Destroy(this.GetComponent<MeshCollider>());
    }
}
