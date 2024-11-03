
using UnityEngine;
using UnityEngine.UI;

public class OutCYL_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_out;
    public GameObject obj_outcyl;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_out.onValueChanged.AddListener(UpdatePosition);
        obj_outcyl = GameObject.Find("Out_Cylinder");

        Toggle_out.isOn = obj_outcyl.GetComponent<Out_Cylinder_Controller>().currentValue;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_out.isOn = obj_outcyl.GetComponent<Out_Cylinder_Controller>().currentValue;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_outcyl.GetComponent<Out_Cylinder_Controller>().currentValue = value;
    }
}
