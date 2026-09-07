using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject[] selectablePrefabs;
    [SerializeField] private GameObject[] nonSelectablePrefabs;

    [SerializeField] private int selectableObjectCount = 50;
    [SerializeField] private int nonSelectableObjectCount = 50;

    // Keep objects this far away from the edges
    [SerializeField] private float edgePadding = 2f;
    [SerializeField] private float minSpawnDistance = 2f;
    [SerializeField] private int maxSpawnAttempts = 30;

    private Bounds groundBounds;

    private void Start()
    {
        groundBounds = ground.GetComponent<Renderer>().bounds;

        SpawnAllObjects();
    }

    private void SpawnAllObjects()
    {
        for (int i = 0; i < selectableObjectCount; i++)
        {
            SpawnObjects(selectablePrefabs, true);
        }

        for (int i = 0; i < nonSelectableObjectCount; i++)
        {
            SpawnObjects(nonSelectablePrefabs, false);
        }
    }

    private void SpawnObjects(GameObject[] prefabs, bool isSelectable = true)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;
        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        if (prefab == null)
        {
            Debug.LogWarning("A prefab in the prefab array is null.");
            return;
        }

        Vector3 position;

        int attempts = 0;
        do
        {
            position = GetRandomPointOnGround();
            attempts++;
        }
        while (!IsPositionValid(position) && attempts < maxSpawnAttempts && isSelectable);
        if (attempts >= maxSpawnAttempts)
        {
            Debug.LogWarning("Could not find a valid spawn position.");
            return;
        }

        Quaternion rotation = Quaternion.Euler(
            prefab.transform.rotation.eulerAngles.x,
            Random.Range(0f, 360f),
            prefab.transform.rotation.eulerAngles.z);

        Instantiate(prefab, position, rotation, transform);
    }


    private Vector3 GetRandomPointOnGround()
    {
        float x = Random.Range(
            groundBounds.min.x + edgePadding,
            groundBounds.max.x - edgePadding);

        float z = Random.Range(
            groundBounds.min.z + edgePadding,
            groundBounds.max.z - edgePadding);

        return new Vector3(x, groundBounds.max.y, z);
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(position, child.position);

            if (distance < minSpawnDistance)
            {
                return false;
            }
        }

        return true;
    }

}