using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ResetSimulation : MonoBehaviour
{
    public GameObject gridSizeGO;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnResetButtonPressed()
    {
        GridDrawer.Instance.setGridSize(int.Parse(gridSizeGO.GetComponent<InputField>().text));
        World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(ButtonPressed));
    }
}

public struct ButtonPressed : IComponentData
{

}
