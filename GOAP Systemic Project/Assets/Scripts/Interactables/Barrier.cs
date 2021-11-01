using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public GameObject posA;
    public GameObject posB;
    private GameObject occupyingPos;
    public GameObject owner;

    public bool occupied;

    private LayerMask coverLayer;

    // Start is called before the first frame update
    void Start()
    {
        coverLayer = 1 << 10;
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public GameObject GetCoverPos()
    {
        GameObject player = FindObjectOfType<PlayerBehaviour>().gameObject;

        if (Physics.Linecast(posA.transform.position, player.transform.position, coverLayer))
        {
            occupyingPos = posA;
            return posA;
        }
        else
        {
            occupyingPos = posB;
            return posB;
        }
    }

    public bool GetCoverStatus()
    {
        GameObject player = FindObjectOfType<PlayerBehaviour>().gameObject;

        if (Physics.Linecast(occupyingPos.transform.position, player.transform.position, coverLayer))
            return true;
        else
            return false;
    }
}