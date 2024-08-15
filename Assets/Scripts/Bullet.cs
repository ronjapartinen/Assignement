using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    NetworkVariable<Vector2> moveValue = new NetworkVariable<Vector2>();

    [SerializeField] private float speed = 5;
   public GameObject owner;

    public Rigidbody2D rb;

    Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        OnShoot();

        
    }

    public void OnShoot()
    {
        BulletMoveRPC(transform.up * speed);
    }
   
    void Update()
    {
        
        if (IsServer)
        {
            Debug.Log("Moving");

            transform.position += (Vector3)moveValue.Value * Time.deltaTime;
        }
      
      //  transform.position += transform.up * speed * Time.deltaTime;
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            DestroyBulletRPC();
        }
    }

    [Rpc(SendTo.Server)]
    private void BulletMoveRPC(Vector2 data)
    {
        moveValue.Value = data;
    }

    [Rpc(SendTo.Server)]
    private void DestroyBulletRPC()
    {
        Destroy(gameObject);
    }
}
