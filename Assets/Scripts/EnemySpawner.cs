using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [Header("Gizmo Settings")]
    [SerializeField] private Color gizmoColor = Color.yellow;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1; //Might be better to say how long it should take for each enemy to spawn, but this is fine for now. But the other way might lead to more control of game flow
    [SerializeField] private int enemiesToSpawn = 10;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject enemyPrefabs; //TODO: if I want to spawn more then one enemy type, i should make this an array and se to how enemiesToSpawn should be set

    [Header("Spawn Dimensions")]
    [SerializeField] private Vector3 spawnDimensions = new Vector3(10, 10, 10);
    [SerializeField] private Vector3 rotation = new Vector3(0, 0, 0);
    private Vector3 position;
    private Quaternion quaternionRotation;

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, spawnDimensions);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
        Gizmos.DrawCube(Vector3.zero, spawnDimensions);
    }

    void Start()
    {
        position = transform.position;
        quaternionRotation = Quaternion.Euler(rotation);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Invoke("SpawnEnemies", spawnInterval * i);
        }
    }

    private void SpawnEnemies()
    {
        Vector3 randomCubeLocation = new Vector3(Random.Range(-spawnDimensions.x / 2, spawnDimensions.x / 2), Random.Range(-spawnDimensions.y / 2, spawnDimensions.y / 2), Random.Range(-spawnDimensions.z / 2, spawnDimensions.z / 2));
        Vector3 spawnLocation = position + quaternionRotation * randomCubeLocation;
        Instantiate(enemyPrefabs, spawnLocation, Quaternion.identity);
    }
}
