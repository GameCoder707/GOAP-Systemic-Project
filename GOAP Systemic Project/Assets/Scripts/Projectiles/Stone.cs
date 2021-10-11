using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private float lifeTime;

    private int damage;
    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 3.0f;
        damage = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime <= 0)
            ResetObj();
        else
            lifeTime -= Time.deltaTime;
    }

    private void ResetObj()
    {
        lifeTime = 3.0f;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerBehaviour>() != null)
        {
            other.gameObject.GetComponent<PlayerBehaviour>().InflictDamage(3);
        }

        ResetObj();
    }
}
