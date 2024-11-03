
using UnityEngine;
using UnityEngine.UI;

public class GantryGIP_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_gripper;
    public GameObject obj_GantryGIP;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_gripper.onValueChanged.AddListener(UpdatePosition);
        obj_GantryGIP = GameObject.Find("Gantry_gripper");

        Toggle_gripper.isOn = obj_GantryGIP.GetComponent<Gantry_Gripper_Controller>().Grip;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_gripper.isOn = obj_GantryGIP.GetComponent<Gantry_Gripper_Controller>().Grip; 
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_GantryGIP.GetComponent<Gantry_Gripper_Controller>().Grip= value;
    }
}
