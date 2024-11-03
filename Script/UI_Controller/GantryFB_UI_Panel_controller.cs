
using UnityEngine;
using UnityEngine.UI;

public class GantryFB_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_FB;
    public GameObject obj_GantryFB;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_FB.onValueChanged.AddListener(UpdatePosition);
        obj_GantryFB = GameObject.Find("Gantry_Forward/Backward");

        Toggle_FB.isOn = obj_GantryFB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_FB.isOn = obj_GantryFB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_GantryFB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = value;
    }
}
