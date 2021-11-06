using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXLife : MonoBehaviour
{
    public float lifeTime;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            timer = lifeTime;
            Destroy(gameObject);
        }
        else
            timer -= Time.deltaTime;
    }
}