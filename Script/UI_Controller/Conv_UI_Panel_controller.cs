
using UnityEngine;
using UnityEngine.UI;

public class Conv_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_conv1;
    public Toggle Toggle_conv2;
    private SystemManager systemManager;

    void Start()
    {
        systemManager = FindObjectOfType<SystemManager>();

        Toggle_conv1.onValueChanged.AddListener(UpdatePosition1);
        Toggle_conv2.onValueChanged.AddListener(UpdatePosition2);



    }

   

    void UpdatePosition1(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        systemManager.WriteModbusCoil(201, value);
    }
    void UpdatePosition2(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        systemManager.WriteModbusCoil(202, value);
    }



}
