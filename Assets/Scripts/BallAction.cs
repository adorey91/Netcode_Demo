using UnityEngine;

public class BallAction : MonoBehaviour
{
    private PlayerMovement player;
    [SerializeField] private float lifetime = 5f;
    private float timer;
    internal GameObject owner;
    private Rigidbody rb;
    private float speed = 10f;

    private void Start()
    {
        timer = 0;
        rb = GetComponent<Rigidbody>();
    }

    internal void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    private void Update()
    {
        if(timer >= lifetime)
            Destroy(this.gameObject);
        else
            timer += Time.deltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if(obj.CompareTag("Player") && obj != owner)
        {
            obj.GetComponent<HealthSystem>().TakeDamage();
            Destroy(this);
        }
    }
}
