
using UnityEngine;
using UnityEngine.UI;

public class GantryROT_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_Rotation;
    public GameObject obj_GantryROT;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_Rotation.onValueChanged.AddListener(UpdatePosition);
        obj_GantryROT = GameObject.Find("Gantry_rotator");

        Toggle_Rotation.isOn = obj_GantryROT.GetComponent<ConditionalRotateObject>().currentValue;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_Rotation.isOn = obj_GantryROT.GetComponent<ConditionalRotateObject>().currentValue;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_GantryROT.GetComponent<ConditionalRotateObject>().currentValue = value;
    }
}
