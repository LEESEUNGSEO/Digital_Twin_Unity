
using UnityEngine;
using UnityEngine.UI;

public class Stopper0_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_Stopper0;
    public GameObject obj_stopper0;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_Stopper0.onValueChanged.AddListener(UpdatePosition);
        obj_stopper0 = GameObject.Find("Stopper_0-1");

        Toggle_Stopper0.isOn = obj_stopper0.GetComponent<Stopper0_Controller>().currentValue;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_Stopper0.isOn = obj_stopper0.GetComponent<Stopper0_Controller>().currentValue;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_stopper0.GetComponent<Stopper0_Controller>().currentValue = value;
    }
}
