using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : Entity
{
    public bool playerInRange = false;

    private LayerMask playerLayer = 1 << 7;

    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 50;
        health = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        Collider[] player = Physics.OverlapSphere(transform.position, 15, playerLayer);

        if (player.Length > 0 && electricPoints < maxElectricPoints)
            playerInRange = true;
        else
            playerInRange = false;

        if(!isHealthy())
        {
            health += Time.deltaTime;
        }

        StatusEffects();
        DisplayUIElements();

    }

    private void LateUpdate()
    {
        canvas.transform.position = transform.position + (Vector3.up * 2f);
    }

    public bool isHealthy()
    {
        if (health >= 0.5 * maxHealth)
            return true;
        else
            return false;
    }
}

