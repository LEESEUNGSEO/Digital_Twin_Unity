
using UnityEngine;
using UnityEngine.UI;

public class DisposeCYL_UI_Panel_controller : MonoBehaviour
{
    public Toggle Toggle_dispose;
    public GameObject obj_disposecyl;

    private GameObject selectedObject;

    void Start()
    {
        Toggle_dispose.onValueChanged.AddListener(UpdatePosition);
        obj_disposecyl = GameObject.Find("Dispose_Cylinder");

        Toggle_dispose.isOn = obj_disposecyl.GetComponent<Dispose_Cylinder_Controller>().moveForward;

    }

    public void SetSelectedObject(GameObject obj)
    {
        selectedObject = obj;
        if (selectedObject != null)
        {
            // Correctly setting the toggle value
            Toggle_dispose.isOn = obj_disposecyl.GetComponent<Dispose_Cylinder_Controller>().moveForward;
        }
    }

    void UpdatePosition(bool value)
    {
        // Update the currentValue in ConditionalRotateObject
        obj_disposecyl.GetComponent<Dispose_Cylinder_Controller>().moveForward = value;
    }
}
