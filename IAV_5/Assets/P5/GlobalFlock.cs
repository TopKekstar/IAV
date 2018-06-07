using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public static int SceneSize = 20;
    public static int SceneSizeX = 18;
    public static int SceneTopY = 15;
    public static int SceneBotY = -3;
    public static int SceneSizeZ = 30;

    /// <summary>
    /// Prefab del Fantasma para crealos en el grupo
    /// </summary>
    public GameObject GhostPrefab;

    /// <summary>
    /// Número de peces que crea el Manager
    /// </summary>
    private static int numFish = 20;

    /// <summary>
    /// Array con todos los Fantasmas
    /// </summary>
    public static GameObject[] AllGhosts = new GameObject[numFish];

    /// <summary>
    /// Tiene que estar en el centro de los fantasmas
    /// </summary>
    public static Vector3 GoalPos = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-SceneSizeX, SceneSizeX), Random.Range(SceneBotY, SceneTopY), Random.Range(-SceneSizeZ, SceneSizeZ));
            AllGhosts[i] = Instantiate(GhostPrefab, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0,10000) < 50)
        {
            GoalPos = new Vector3(Random.Range(-SceneSizeX, SceneSizeX), Random.Range(SceneBotY, SceneTopY), Random.Range(SceneSizeZ, SceneSizeZ));
        }
    }
}
