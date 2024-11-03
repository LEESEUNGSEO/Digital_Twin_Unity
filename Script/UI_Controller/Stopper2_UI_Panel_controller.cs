
using UnityEngine;
using UnityEngine.UI;

public class Stopper2_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_Stopper2;
    public GameObject obj_stopper2;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_Stopper2.onValueChanged.AddListener(UpdatePosition);
        obj_stopper2 = GameObject.Find("Stopper_2-1");

        Toggle_Stopper2.isOn = obj_stopper2.GetComponent<Stopper2_Controller>().moveforward;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_Stopper2.isOn = obj_stopper2.GetComponent<Stopper2_Controller>().moveforward;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_stopper2.GetComponent<Stopper2_Controller>().moveforward = value;
    }
}
