using System.Collections.Generic;
using UnityEngine;

public class CableController : MonoBehaviour
{
    public Transform leashStart; 
    public Transform leashEnd;   
    public float segmentLength = 0.25f;
    public float capsuleRadius = 0.02f;

    private GameObject[] segments;

    void Start()
    {
        if (!leashStart || !leashEnd)
        {
            Debug.LogError("Missing assignment. As expected...");
            return;
        }

        List<GameObject> collectedBones = new List<GameObject>();
        CollectBonesRecursive(leashStart, collectedBones);

        int segmentCount = collectedBones.Count;
        if (segmentCount == 0)
        {
            Debug.LogError("No bones found under leashStart.");
            return;
        }

        segments = collectedBones.ToArray();

        Vector3 direction = (leashEnd.position - leashStart.position).normalized;
        Vector3 currentPosition = leashStart.position;

        GameObject previousSegment = null;

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = segments[i];

            Rigidbody rb = segment.GetComponent<Rigidbody>();
            if (!rb)
                rb = segment.AddComponent<Rigidbody>();
            rb.mass = 0.1f;
            rb.drag = 0.2f;

            rb.constraints = RigidbodyConstraints.FreezeRotationX
               | RigidbodyConstraints.FreezeRotationZ;

            ConfigurableJoint joint = segment.GetComponent<ConfigurableJoint>();
            if (!joint)
                joint = segment.AddComponent<ConfigurableJoint>();
            joint.axis = Vector3.forward;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            if (previousSegment != null)
            {
                joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
                joint.anchor = Vector3.zero;
            }
            else
            {
                rb.isKinematic = true;
                segment.transform.position = leashStart.position;
            }

            previousSegment = segment;
            currentPosition += direction * segmentLength;
        }

        Rigidbody dogRb = leashEnd.GetComponent<Rigidbody>();
        if (!dogRb)
        {
            Debug.LogWarning("Your 'dog' has no Rigidbody. It must be very well-trained.");
            dogRb = leashEnd.gameObject.AddComponent<Rigidbody>();
        }

        ConfigurableJoint finalJoint = leashEnd.GetComponent<ConfigurableJoint>();
        if (!finalJoint)
            finalJoint = leashEnd.gameObject.AddComponent<ConfigurableJoint>();

        finalJoint.connectedBody = segments[segmentCount - 1].GetComponent<Rigidbody>();
        finalJoint.xMotion = ConfigurableJointMotion.Locked;
        finalJoint.yMotion = ConfigurableJointMotion.Locked;
        finalJoint.zMotion = ConfigurableJointMotion.Locked;
    }

    void CollectBonesRecursive(Transform current, List<GameObject> bones)
    {
        bones.Add(current.gameObject);
        foreach (Transform child in current)
        {
            CollectBonesRecursive(child, bones);
        }
    }
}