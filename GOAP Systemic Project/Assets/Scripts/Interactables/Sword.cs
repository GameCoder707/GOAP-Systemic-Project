using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    // Start is called before the first frame update
    void Start()
    {
        damage = 7;

        MainStart();
    }

    // Update is called once per frame
    void Update()
    {
        MainUpdate();
    }
}
