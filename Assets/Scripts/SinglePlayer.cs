using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : MonoBehaviour
{
    bool GM_isSinglePlayer = true;
    bool pause = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (!GM_isSinglePlayer)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pause = !pause;

            if(pause)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }
}
