using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public bool playerInRange = false;

    private LayerMask playerLayer = 1 << 7;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] player = Physics.OverlapSphere(transform.position, 15, playerLayer);

        if (player.Length > 0)
            playerInRange = true;
        else
            playerInRange = false;

    }
}

