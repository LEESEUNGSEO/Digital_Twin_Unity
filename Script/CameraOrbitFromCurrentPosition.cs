using UnityEngine;

public class CameraOrbitFromCurrentPosition : MonoBehaviour
{
    public Transform target;           // 카메라가 회전할 타겟 오브젝트
    public float orbitSpeed = 20.0f;   // 회전 속도
    public Vector3 orbitAxis = Vector3.up;  // 회전할 축 (기본적으로 Y축)
    public float smoothFactor = 0.1f;  // 부드러운 이동을 위한 보간 속도

    private bool orbiting = false;     // 카메라가 회전 중인지 여부
    private float currentAngle = 0.0f; // 현재 회전한 각도
    private float distanceToTarget;    // 타겟과의 거리
    private Vector3 targetPosition;    // 카메라의 목표 위치

    void Start()
    {
        // 카메라와 타겟 사이의 거리를 계산
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 타겟을 바라보도록 설정
        transform.LookAt(target);

        // 회전 시작
        StartOrbit();
    }

    void Update()
    {
        if (orbiting)
        {
            // 회전할 각도 계산
            float angleThisFrame = orbitSpeed * Time.deltaTime;

            // 카메라를 타겟 주위로 회전
            transform.RotateAround(target.position, orbitAxis, angleThisFrame);

            // 카메라가 타겟과의 거리를 유지하도록 새로운 목표 위치 계산
            Vector3 direction = (transform.position - target.position).normalized;
            targetPosition = target.position + direction * distanceToTarget;

            // 카메라의 위치를 부드럽게 이동 (Lerp)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothFactor);

            // 카메라가 타겟을 부드럽게 바라보도록 회전 (Slerp)
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothFactor);

            // 회전한 각도 추적
            currentAngle += angleThisFrame;

            if (currentAngle >= 360.0f)
            {
                orbiting = false;
            }
        }
    }

    // 회전을 시작하는 함수
    public void StartOrbit()
    {
        orbiting = true;
        currentAngle = 0.0f; // 초기화
    }
}
