
using UnityEngine;
using UnityEngine.UI;

public class Stopper1_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_Stopper1;
    public GameObject obj_stopper1;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_Stopper1.onValueChanged.AddListener(UpdatePosition);
        obj_stopper1 = GameObject.Find("Stopper_1-1");

        Toggle_Stopper1.isOn = obj_stopper1.GetComponent<Stopper1_Controller>().currentValue;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_Stopper1.isOn = obj_stopper1.GetComponent<Stopper1_Controller>().currentValue;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_stopper1.GetComponent<Stopper1_Controller>().currentValue = value;
    }
}
