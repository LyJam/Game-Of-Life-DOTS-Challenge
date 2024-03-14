using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerformanceData : MonoBehaviour
{
    public GameObject fps;
    public GameObject simulationSystem;
    public GameObject drawSystem;

    private Text fpsText;
    private Text simulationSystemText;
    private Text drawSystemText;

    public static PerformanceData Instance { private set; get; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fpsText = fps.GetComponent<Text>();
        simulationSystemText = simulationSystem.GetComponent<Text>();
        drawSystemText = drawSystem.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float fps = (1f/Time.deltaTime);
        fpsText.text =  "FPS: " + fps.ToString("F2");
    }

    public void setSimulationTime(float time)
    {
        simulationSystemText.text = "Simulation time: " + time.ToString() + " ms";
    }

    public void setDrawTime(float time)
    {
        drawSystemText.text = "draw time: " + time.ToString() + " ms";
    }
}
