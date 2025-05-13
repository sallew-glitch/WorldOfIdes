using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class FloatingIsland : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 20, 0); // How high to float
    public float floatSpeed = 2f; // Speed of floating
    public float delay = 5f; // Delay before floating starts

    public NavMeshSurface navMeshSurface; // Assign this in Inspector
    public GameObject objectToEnableAfterBake; // Assign in Inspector

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool floating = false;
    private bool navMeshBaked = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + offset;
        Invoke(nameof(StartFloating), delay);
    }

    void StartFloating()
    {
        floating = true;
    }

    void Update()
    {
        if (floating && !navMeshBaked)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, floatSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                floating = false;
                navMeshBaked = true;
                BakeNavMesh();
            }
        }
    }

    void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }

        if (objectToEnableAfterBake != null)
        {
            objectToEnableAfterBake.SetActive(true);
        }
    }
}
