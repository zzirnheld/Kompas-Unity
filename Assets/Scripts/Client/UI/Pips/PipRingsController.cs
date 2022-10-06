using UnityEngine;

public class PipRingsController : MonoBehaviour
{
    //Set in inspector:
    public GameObject[] rings;
    public float torqueIntensity;

    private int numToShow = 0;

    private Rigidbody[] ringRigidbodies;

    private void Awake()
    {
        //Set up the variables according to the number of rings set in the inspector.
        ringRigidbodies = new Rigidbody[rings.Length];
        for (int i = 0; i < rings.Length; i++)
        {
            ringRigidbodies[i] = rings[i].GetComponent<Rigidbody>();
        }

        InvokeRepeating(nameof(AddRandomForce), 0.0f, 0.5f);
    }

    private void AddRandomForce()
    {
        for (int i = 0; i < numToShow; i++)
        {
            //ringRigidbodies[i].AddRelativeTorque(rings[i].transform.up * torqueIntensity * (Random.value - 0.5f));
            ringRigidbodies[i].AddRelativeTorque(Vector3.up * torqueIntensity * (Random.value - 0.5f));
            ringRigidbodies[i].AddRelativeTorque(Vector3.left * torqueIntensity * (Random.value - 0.5f));
        }
    }

    public void ShowRings(int numToShow)
    {
        //Debug.Log($"Showing {numToShow} rings");
        transform.rotation = Quaternion.identity;
        //Move last active ring down
        foreach (var ring in rings)
        {
            ring.transform.localPosition = Vector3.zero;
        }

        //Slow down any newly inactive rings
        for (int i = numToShow; i < rings.Length; i++)
        {
            ringRigidbodies[i].angularVelocity = Vector3.zero;
            rings[i].transform.localRotation = Quaternion.identity;
        }

        this.numToShow = numToShow;
        //Move active ring(s) up
        if (numToShow > 0) rings[numToShow - 1].transform.localPosition = Vector3.back * 10;
    }
}
