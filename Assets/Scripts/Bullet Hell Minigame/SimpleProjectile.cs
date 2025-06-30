using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}