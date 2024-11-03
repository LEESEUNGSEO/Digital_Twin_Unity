
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public GameObject Conv_uiPanel;   public GameObject GantryXY_uiPanel; // UI 패널 연결
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
    public Button targetButton; // 특정 버튼을 Inspector에서 설정

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 체크
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 위에 있는지 확인
            if (IsPointerOverUIElement(targetButton))
            {
                // 특정 버튼 위에서 Raycast 차단
                Debug.Log("Pointer is over the target button, stopping Raycast.");
                return;
            }

            // Main Camera를 찾아서 오브젝트 선택
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

        // Raycast로 오브젝트 감지
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
                                    Debug.LogError("GantryGIPTargetCamTransform이 설정되지 않았습니다.");
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
        }// UI 패널 활성화
        
    }


    bool HasParentWithTag(GameObject obj, string tag)
    {
        Transform current = obj.transform;

        // 부모 객체들을 위로 거슬러 올라가며 탐색
        while (current.parent != null)
        {
            current = current.parent;
            if (current.CompareTag(tag))
            {
                return true; // 특정 태그를 가진 부모가 있으면 true 반환
            }
        }
        return false; // 없으면 false 반환
    }





    bool IsPointerOverUIElement(Button targetButton)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition // 마우스 위치 설정
        };

        // UI 요소와 Raycast 결과를 저장하는 리스트
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        // Raycast된 UI 요소 중에서 targetButton의 GameObject가 있는지 확인
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == targetButton.gameObject) // 여기서 gameObject 접근
            {
                return true; // 특정 버튼 위에 있을 때 true 반환
            }
        }
        return false; // 특정 버튼 위에 없을 때 false 반환
    }



}
