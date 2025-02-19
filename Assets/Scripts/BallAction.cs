using UnityEngine;

public class BallAction : MonoBehaviour
{
    private PlayerMovement player;
    [SerializeField] private float lifetime = 5f;
    private float timer;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if(timer >= lifetime)
        {
            Destroy(this);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if(obj.CompareTag("Player"))
        {
            obj.GetComponent<HealthSystem>().TakeDamage();
            Destroy(this);
        }
    }
}
