using UnityEngine;

public class CameraOrbitFromCurrentPosition : MonoBehaviour
{
    public Transform target;           // ī�޶� ȸ���� Ÿ�� ������Ʈ
    public float orbitSpeed = 20.0f;   // ȸ�� �ӵ�
    public Vector3 orbitAxis = Vector3.up;  // ȸ���� �� (�⺻������ Y��)
    public float smoothFactor = 0.1f;  // �ε巯�� �̵��� ���� ���� �ӵ�

    private bool orbiting = false;     // ī�޶� ȸ�� ������ ����
    private float currentAngle = 0.0f; // ���� ȸ���� ����
    private float distanceToTarget;    // Ÿ�ٰ��� �Ÿ�
    private Vector3 targetPosition;    // ī�޶��� ��ǥ ��ġ

    void Start()
    {
        // ī�޶�� Ÿ�� ������ �Ÿ��� ���
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Ÿ���� �ٶ󺸵��� ����
        transform.LookAt(target);

        // ȸ�� ����
        StartOrbit();
    }

    void Update()
    {
        if (orbiting)
        {
            // ȸ���� ���� ���
            float angleThisFrame = orbitSpeed * Time.deltaTime;

            // ī�޶� Ÿ�� ������ ȸ��
            transform.RotateAround(target.position, orbitAxis, angleThisFrame);

            // ī�޶� Ÿ�ٰ��� �Ÿ��� �����ϵ��� ���ο� ��ǥ ��ġ ���
            Vector3 direction = (transform.position - target.position).normalized;
            targetPosition = target.position + direction * distanceToTarget;

            // ī�޶��� ��ġ�� �ε巴�� �̵� (Lerp)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFactor);

            // ī�޶� Ÿ���� �ε巴�� �ٶ󺸵��� ȸ�� (Slerp)
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothFactor);

            // ȸ���� ���� ����
            currentAngle += angleThisFrame;

            if (currentAngle >= 360.0f)
            {
                orbiting = false;
            }
        }
    }

    // ȸ���� �����ϴ� �Լ�
    public void StartOrbit()
    {
        orbiting = true;
        currentAngle = 0.0f; // �ʱ�ȭ
    }
}
