using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;


public class Player : NetworkBehaviour
{

    [SerializeField] InputReader inputReader;
    NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>();
    NetworkVariable<Vector3> mousePos = new NetworkVariable<Vector3>();
    [SerializeField] GameObject bulletToSpawn;
    [SerializeField] Transform firingPoint;
    public Vector3 mouse;
    public GameObject col;

    public float MaxHealth = 1;
    public float currentHealth;

    
    void Start()
    {
        if (inputReader != null && IsLocalPlayer)
        {
            inputReader.MoveEvent += OnMove;
            inputReader.ShootEvent += OnShootRPC;
        
        }
        currentHealth = MaxHealth;
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }

    void Update()
    {
        if (IsServer)
        {
            transform.position += (Vector3)moveInput.Value / 10;         
            float angle = Mathf.Atan2(mousePos.Value.y - transform.position.y, mousePos.Value.x - transform.position.x) * Mathf.Rad2Deg - 90;
            transform.localRotation = Quaternion.Euler(0, 0, angle);
            
        }

        if(IsLocalPlayer)
        {
            MousePosRPC(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
     
    }

    [Rpc(SendTo.Server)]
    private void MousePosRPC(Vector3 data)
    {

        mousePos.Value = data;
    }


    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }

    [Rpc(SendTo.Server)]
    private void OnShootRPC()
    {
        Debug.Log("Shooting");
        NetworkObject obj = Instantiate(bulletToSpawn, firingPoint.position, firingPoint.rotation).GetComponent<NetworkObject>();
        obj.gameObject.GetComponent<Bullet>().owner = gameObject;
        Debug.Log(firingPoint.position);
        obj.Spawn();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided");
        if( collision.gameObject.tag == "Bullet" && collision.gameObject.GetComponent<Bullet>().owner != gameObject)
        {
            col = collision.gameObject;
            TakeDamageRPC(1);
            Debug.Log("Hitt");
        }

    }

   

    [Rpc(SendTo.ClientsAndHost)]
    public void TakeDamageRPC(int amount)
    {

        currentHealth -= amount;


        if (currentHealth <= 0)
        {
             DeactiveRPC();
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeactiveRPC()
    {     
      if(col.GetComponent<Bullet>().owner != gameObject)
        {

            gameObject.SetActive(false);
          
        }
      
    }

}