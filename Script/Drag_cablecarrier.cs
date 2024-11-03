using UnityEngine;

public class Drag_cablecarrier : MonoBehaviour
{
    public Transform childObject; // �ڽ� ��ü�� �巡���� �� �̵���ų �ڽ� ��ü

    private bool isDragging = false;
    private float distanceToCamera;
    private Vector3 previousMousePosition;

    void Start()
    {
        // ī�޶���� �ʱ� �Ÿ� ���
        distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
    }

    void Update()
    {
        // ���콺 ���� ��ư�� ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 ��ġ�� �̿��� ����ĳ��Ʈ
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isDragging = true;
                previousMousePosition = Input.mousePosition;
            }
        }

        // ���콺 ���� ��ư�� ���� ��
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // �巡�� ���� �� ������Ʈ ��ġ ������Ʈ
        if (isDragging)
        {
            // ���콺 ��ġ�� Z�� �������� ��ȯ
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 point = ray.GetPoint(distanceToCamera);

            // �θ� ��ü�� �̵� ���
            Vector3 parentNewPosition = new Vector3(transform.position.x, transform.position.y, point.z);
            Vector3 parentDelta = parentNewPosition - transform.position;

            // �θ� ��ü �̵�
            transform.position = parentNewPosition;

            // �ڽ� ��ü �̵� (�θ� �̵��� ����)
            if (childObject != null)
            {
                Vector3 childNewPosition = childObject.position - parentDelta * 0.5f;
                childObject.position = new Vector3(childObject.position.x, childObject.position.y, childNewPosition.z);
            }
        }
    }
}