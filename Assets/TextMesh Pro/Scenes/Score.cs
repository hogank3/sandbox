using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public float timer = 0f;
    public Text display;

    bool scoreActive = true;

    // Start is called before the first frame update
    void Start()
    {
        display.text = "Score: " + timer.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        display.text = "Score: " + Mathf.Round(timer).ToString();
    }

    public void pauseScore()
    {
        scoreActive = !scoreActive;
    }
}
