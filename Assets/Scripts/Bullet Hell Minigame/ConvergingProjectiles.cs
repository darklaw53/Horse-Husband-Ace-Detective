using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConvergingProjectiles : MonoBehaviour
{
    [Header("Settings")]
    public List<Transform> spawnPoints;
    public List<GameObject> projectilePrefabs;
    public float spawnInterval = 1f;
    public Transform centerPoint;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Count < 3)
        {
            Debug.LogError("You need at least *three* spawn points, Master. Don’t make me repeat myself.");
            return;
        }

        if (projectilePrefabs == null || projectilePrefabs.Count == 0)
        {
            Debug.LogError("You must assign at least one projectile prefab, Master. Please.");
            return;
        }

        StartCoroutine(SpawnProjectiles());
    }

    private IEnumerator SpawnProjectiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Transform basePoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            List<Transform> neighbors = spawnPoints
                .Where(p => p != basePoint)
                .OrderBy(p => Vector3.Distance(p.position, basePoint.position))
                .Take(2)
                .ToList();

            Transform neighbor = neighbors[Random.Range(0, neighbors.Count)];

            float t = Random.Range(0f, 1f);
            Vector3 spawnPos = Vector3.Lerp(basePoint.position, neighbor.position, t);

            Vector3 direction = (centerPoint.position - spawnPos).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject chosenProjectile = projectilePrefabs[Random.Range(0, projectilePrefabs.Count)];
            Instantiate(chosenProjectile, spawnPos, rotation);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (spawnPoints != null && spawnPoints.Count > 1)
        {
            Vector3 center = Vector3.zero;
            foreach (Transform point in spawnPoints)
            {
                Gizmos.DrawSphere(point.position, 0.1f);
                center += point.position;
            }
            center /= spawnPoints.Count;
            Gizmos.DrawWireSphere(center, 0.3f);
        }
    }
#endif
}