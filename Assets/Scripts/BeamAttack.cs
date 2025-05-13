using UnityEngine;

public class BeamAttack : MonoBehaviour
{
    public float speed = 15f;
    public float damage = 40f;
    public GameObject impactVFX; // VFX prefab for impact
    public float lifetime = 3f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Optional: Look at player on spawn
        Vector3 direction = (player.position + new Vector3(0, 1f, 0)) - transform.position;
        transform.rotation = Quaternion.LookRotation(direction.normalized);

        Destroy(gameObject, lifetime); // Self-destroy after a while
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // Play impact VFX
            if (impactVFX != null)
            {
                Instantiate(impactVFX, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
