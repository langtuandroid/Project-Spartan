using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPresencePhysics : MonoBehaviour
{
    public Transform target;
    public Transform master;
    private Rigidbody rb;
    public Renderer nonPhysicalHand;
    public float showNonPhysicalHandDistance = .05f;
    private Collider[] handColliders;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        handColliders = GetComponentsInChildren<Collider>();
    }

    public void EnableHandColliders()
    {
        foreach (var item in handColliders)
        {
            item.enabled = true;
        }
    }

    public void EnableColliderDelay(float delay)
    {
        Invoke("EnableHandColliders", delay);
    }

    public void DisableHandColliders()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > showNonPhysicalHandDistance)
        {
            nonPhysicalHand.enabled = true;
        }
        else
            nonPhysicalHand.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = (target.position - transform.position)/Time.fixedDeltaTime;

        Quaternion rotationDifference = master.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

        rb.angularVelocity = (rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime);
    }
}
