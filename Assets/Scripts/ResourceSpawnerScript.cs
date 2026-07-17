using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private int objectCount = 100;

    // Keep objects this far away from the edges
    [SerializeField] private float edgePadding = 2f;

    private Bounds groundBounds;

    private void Start()
    {
        groundBounds = ground.GetComponent<Renderer>().bounds;

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 position = GetRandomPointOnGround();

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

            Quaternion rotation = Quaternion.Euler(
                prefab.transform.rotation.eulerAngles.x,
                Random.Range(0f, 360f),
                prefab.transform.rotation.eulerAngles.z);

            Instantiate(prefab, position, rotation, transform);
        }
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
}