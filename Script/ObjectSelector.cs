
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public GameObject Conv_uiPanel;   public GameObject GantryXY_uiPanel; // UI �г� ����
    public GameObject GantryROT_uiPanel;
    public GameObject GantryFB_uiPanel;
    public GameObject GanetryGIP_uiPanel;
    public GameObject Stopper0_uiPanel;
    public GameObject Stopper1_uiPanel;
    public GameObject Stopper2_uiPanel;
    public GameObject Dispose_cyl_uiPanel;
    public GameObject Out_cyl_uiPanel;

    public GameObject UIbackground;



    public Transform GantryXYTarget_CamTransform;
    public Transform GantryROTTargetCamTransform;
    public Transform GantryFBTargetCamTransform;
    public Transform GantryGIPTargetCamTransform;
    public Transform Stopper0TargetCamTransform;
    public Transform Stopper1TargetCamTransform;
    public Transform Stopper2TargetCamTransform;
    public Transform DisposeTargetCamTransform;
    public Transform OutTargetCamTransform;


    public GameObject ROT_CAM_SET;
    


    private void Start()
    {
        GameObject UIbackground = GameObject.FindWithTag("UI_background");

    }
    public Button targetButton; // Ư�� ��ư�� Inspector���� ����

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� üũ
        if (Input.GetMouseButtonDown(0))
        {
            // UI ��� ���� �ִ��� Ȯ��
            if (IsPointerOverUIElement(targetButton))
            {
                // Ư�� ��ư ������ Raycast ����
                Debug.Log("Pointer is over the target button, stopping Raycast.");
                return;
            }

            // Main Camera�� ã�Ƽ� ������Ʈ ����
            GameObject mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera != null)
            {
                SelectObject(mainCamera);
            }
            else
            {
                Debug.LogError("MainCamera object not found");
            }
        }
    }


    void SelectObject(GameObject mainCamera)
    {
        FreeCameraController cameraController = mainCamera.GetComponent<FreeCameraController>();
     
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast�� ������Ʈ ����
        if (Physics.Raycast(ray, out hit, 2000.0f))
        {
            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;

                GameObject ROT_CAM_SET = GameObject.FindWithTag("Gantry_Rotator_Tag2");
                if (ROT_CAM_SET != null)
                {
                    ConditionalRotateObject rotateObject = ROT_CAM_SET.GetComponent<ConditionalRotateObject>();
                    if (rotateObject != null)
                    {
                        if (rotateObject.currentValue)
                        {
                            cameraController.targetPosition = new Vector3(-733, 1315, 666);
                        }
                        else
                        {
                            cameraController.targetPosition = new Vector3(-123, 1093, 576);
                        }
                    }
                    else
                    {
                        Debug.LogError("ConditionalRotateObject component is missing from ROT_CAM_SET");
                    }
                }
                else
                {
                    Debug.LogError("Gantry_Rotator_Tag object not found");
                }
                



                if (HasParentWithTag(clickedObject, "XY_Axis_Tag"))
                {
                    if (HasParentWithTag(clickedObject, "Gantry_Rotator_Tag")||HasParentWithTag(clickedObject, "Gantry_Rotator_Tag2"))
                    {
                        if (HasParentWithTag(clickedObject, "Gantry_FB_Tag"))
                        {
                            if (HasParentWithTag(clickedObject, "Gantry_GIP_Tag"))
                            {
                                ShowUIForSelectedObject(GanetryGIP_uiPanel);
                                Debug.Log($"!!!!:{ROT_CAM_SET.GetComponent<ConditionalRotateObject>().currentValue}");
                                if (ROT_CAM_SET.GetComponent<ConditionalRotateObject>().currentValue)
                                {
                                    cameraController.targetPosition = new Vector3(-123, 1093, 576);
                                }
                                else
                                {
                                    cameraController.targetPosition = new Vector3(-733, 1315, 666);
                                }

                                if (GantryGIPTargetCamTransform == null)
                                {
                                    Debug.LogError("GantryGIPTargetCamTransform�� �������� �ʾҽ��ϴ�.");
                                    return;
                                }
                                cameraController.targetObject = GantryGIPTargetCamTransform;

                            }
                            else
                            {
                                ShowUIForSelectedObject(GantryFB_uiPanel);
                                cameraController.targetPosition = new Vector3(-659, 1115, 337);
                                cameraController.targetObject = GantryFBTargetCamTransform;
                            }
                        }
                        else
                        {
                            ShowUIForSelectedObject(GantryROT_uiPanel);
                            cameraController.targetPosition = new Vector3(-653, 1338, 490);
                            cameraController.targetObject = GantryROTTargetCamTransform;
                        }
                    }
                    else
                    {
                        ShowUIForSelectedObject(GantryXY_uiPanel);
                        cameraController.targetPosition = new Vector3(-236, 1385, 407);
                        cameraController.targetObject = GantryXYTarget_CamTransform;
                    }
                }

                if (HasParentWithTag(clickedObject, "Stopper0_Tag")){
                    ShowUIForSelectedObject(Stopper0_uiPanel);
                    cameraController.targetPosition = new Vector3(-597, 1082, 500);
                    cameraController.targetObject = Stopper0TargetCamTransform;
                }


                if (HasParentWithTag(clickedObject, "Stopper1_Tag"))
                {
                    ShowUIForSelectedObject(Stopper1_uiPanel);
                    cameraController.targetPosition = new Vector3(-597, 1082, 373);
                    cameraController.targetObject = Stopper1TargetCamTransform;
                }

                if (HasParentWithTag(clickedObject, "Stopper2_Tag"))
                {
                    ShowUIForSelectedObject(Stopper2_uiPanel);
                    cameraController.targetPosition = new Vector3(-296, 942, 763);
                    cameraController.targetObject = Stopper2TargetCamTransform;
                }

                if (HasParentWithTag(clickedObject, "Dispose_CYL_Tag"))
                {
                    ShowUIForSelectedObject(Dispose_cyl_uiPanel);
                    cameraController.targetPosition = new Vector3(-218, 938, 816);
                    cameraController.targetObject = DisposeTargetCamTransform;
                }

                if (HasParentWithTag(clickedObject, "Out_CYL_Tag"))
                {
                    ShowUIForSelectedObject(Out_cyl_uiPanel);
                    cameraController.targetPosition = new Vector3(-23, 1130, 258);
                    cameraController.targetObject = OutTargetCamTransform;
                }

                if (HasParentWithTag(clickedObject, "CONV_Tag"))
                {
                    ShowUIForSelectedObject(Conv_uiPanel);
                }


            }
        }
    }

    

    void ShowUIForSelectedObject(GameObject uiPanel)
    {
        if (uiPanel != null)
        {
            UIbackground.SetActive(true);
            uiPanel.SetActive(true);
        }// UI �г� Ȱ��ȭ
        
    }


    bool HasParentWithTag(GameObject obj, string tag)
    {
        Transform current = obj.transform;

        // �θ� ��ü���� ���� �Ž��� �ö󰡸� Ž��
        while (current.parent != null)
        {
            current = current.parent;
            if (current.CompareTag(tag))
            {
                return true; // Ư�� �±׸� ���� �θ� ������ true ��ȯ
            }
        }
        return false; // ������ false ��ȯ
    }





    bool IsPointerOverUIElement(Button targetButton)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition // ���콺 ��ġ ����
        };

        // UI ��ҿ� Raycast ����� �����ϴ� ����Ʈ
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        // Raycast�� UI ��� �߿��� targetButton�� GameObject�� �ִ��� Ȯ��
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == targetButton.gameObject) // ���⼭ gameObject ����
            {
                return true; // Ư�� ��ư ���� ���� �� true ��ȯ
            }
        }
        return false; // Ư�� ��ư ���� ���� �� false ��ȯ
    }



}
