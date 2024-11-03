using UnityEngine;

public class Drag_cablecarrier : MonoBehaviour
{
    public Transform childObject; // 자식 객체를 드래그할 때 이동시킬 자식 객체

    private bool isDragging = false;
    private float distanceToCamera;
    private Vector3 previousMousePosition;

    void Start()
    {
        // 카메라와의 초기 거리 계산
        distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
    }

    void Update()
    {
        // 마우스 왼쪽 버튼이 눌렸을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치를 이용해 레이캐스트
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isDragging = true;
                previousMousePosition = Input.mousePosition;
            }
        }

        // 마우스 왼쪽 버튼을 뗐을 때
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 드래그 중일 때 오브젝트 위치 업데이트
        if (isDragging)
        {
            // 마우스 위치를 Z축 방향으로 변환
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 point = ray.GetPoint(distanceToCamera);

            // 부모 객체의 이동 계산
            Vector3 parentNewPosition = new Vector3(transform.position.x, transform.position.y, point.z);
            Vector3 parentDelta = parentNewPosition - transform.position;

            // 부모 객체 이동
            transform.position = parentNewPosition;

            // 자식 객체 이동 (부모 이동의 절반)
            if (childObject != null)
            {
                Vector3 childNewPosition = childObject.position - parentDelta * 0.5f;
                childObject.position = new Vector3(childObject.position.x, childObject.position.y, childNewPosition.z);
            }
        }
    }
}