using UnityEngine;
using UnityEngine.UI;

public class Close_panel : MonoBehaviour
{
    // 비활성화하고 싶은 패널을 public으로 선언하여 Unity Editor에서 드래그하여 설정할 수 있게 함
    public GameObject panelToDisable;
    private GameObject mainCamera;

    private void Start()
    {
        // 전역 변수 mainCamera에 값을 할당
        mainCamera = GameObject.FindWithTag("MainCamera");

        // mainCamera가 제대로 할당되지 않았을 경우를 대비한 에러 체크
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera 오브젝트를 찾을 수 없습니다.");
        }
    }

    // 이 함수는 버튼의 OnClick 이벤트에 연결됨
    public void OnButtonClicked()
    {
        Transform grandparentObject = transform.parent.parent;

        // 패널을 비활성화하는 코드
        panelToDisable.SetActive(false);
        Debug.Log($"{grandparentObject}이거 꺼진다");

        // 부모 오브젝트도 비활성화
        mainCamera.GetComponent<FreeCameraController>().targetObject = null;
        foreach (Transform child in grandparentObject.transform)
        {
            child.gameObject.SetActive(false);
        }


    }
}
