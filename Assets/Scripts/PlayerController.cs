using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    float movementSpeed = 3f; // Unity-enheter per sekund

    [SerializeField]
    float rotationSpeed = 150f; // Grader per sekund

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    Transform bulletSpawnPoint;

    [SerializeField]
    float timeBetweenShots = 0.5f;
    float timeSinceLastShot = 0f;

    bool burst = false;
    float burstTimer = 0.1f;
    int shots = 0;
    bool bursting = false;

    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        float yRotation = Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime;
        float zMovement = Input.GetAxisRaw("Vertical") * movementSpeed * Time.deltaTime;

        Vector3 rotationVector = new Vector3(0, yRotation, 0);
        Vector3 movementVector = new Vector3(0, 0, zMovement);

        transform.Rotate(rotationVector);
        transform.Translate(movementVector);

        timeSinceLastShot += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F))
        {
            burst = !burst;
        }
        if (burst)
        {
            if (bursting)
            {
                if (timeSinceLastShot > burstTimer && shots < 3)
                {
                    Fire();
                    shots++;
                    timeSinceLastShot = 0;
                }
                else if (shots == 3)
                {
                    shots = 0;
                    bursting = false;
                }
            }
            else if (Input.GetAxisRaw("Fire1") > 0)
            {
                if (timeSinceLastShot > timeBetweenShots)
                {
                    //Fire(); 
                    bursting = true;
                    timeSinceLastShot = 0;
                }
            }
        }
        else
        {
            if (Input.GetAxisRaw("Fire1") > 0)
            {
                if (timeSinceLastShot > timeBetweenShots)
                {
                    Fire();
                    timeSinceLastShot = 0;
                }
            }
        }

    }

    [Command]
    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        NetworkServer.Spawn(bullet);
    }
}
