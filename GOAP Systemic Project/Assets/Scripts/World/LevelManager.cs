using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private PlayerBehaviour player;

    public GameObject gameOverScreen;
    private float delay;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerBehaviour>();

        delay = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Main Menu");

        if(player.health <= 0)
        {
            if (delay <= 0.0f)
                gameOverScreen.SetActive(true);
            else
                delay -= Time.deltaTime;
        }
    }
}