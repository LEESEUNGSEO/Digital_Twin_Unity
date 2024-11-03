using UnityEngine;

public class ParentChildOscillateZ : MonoBehaviour
{
    private SystemManager systemManager;

    private int intinputvalue;

    public Transform childObject; // 자식 객체를 이동시킬 자식 객체

    // 0에서 100 사이의 입력 값
    public float inputValue = 0.0f;

    // 부모 객체가 이동할 수 있는 Z 좌표의 최소값과 최대값
    public float minZPosition = 1185.0f;
    public float maxZPosition = 970.0f;    

    // 이동 속도
    private float moveSpeed = 90.0f;

    private float targetZPosition;
    private Vector3 targetChildPosition;

    void Start()
    {

        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
        }
        
        // 초기 목표 위치 설정
        targetZPosition = transform.position.z;
        if (childObject != null)
        {
            targetChildPosition = childObject.position;
        }
    }

    void Update()
    {
        switch (systemManager.CurrentMode)
        {
            case OperationMode.DigitalTwinMode:
                RunDigitalTwin();
                break;

            case OperationMode.SimulationMode:
                RunSimulation();
                break;

            case OperationMode.ManualMode:
                RunManualControl();
                break;
        }
    }

    void RunDigitalTwin()
    {
        readPos();
    }

    void RunSimulation()
    {
        writeMyPos();
    }

    private void RunManualControl()
    {
        writeMyPos();
    }

    private void readPos()
    {
        int registerValue = systemManager.ReadModbusRegister(1103, 1);

        switch (registerValue)
        {
            case 0:
                // null 처리
                break;
            case 1:
                // Zero Position
                inputValue = 0;
                break;
            case 2:
                // Clamp Position
                inputValue = 95167;
                break;
            case 3:
                // Clamp up Position
                inputValue = 95167;
                break;
            case int n when (n >= 20 && n <= 38):
                inputValue = Basket_Controller.coordination[n % 10, 0];
                break;
            default:
                // 위의 값들과 일치하지 않는 경우
                break;
        }
        inputValue = Mathf.Clamp(inputValue, 0.0f, 247280.0f);

        // 입력 값을 0~100 범위에서 minZPosition~maxZPosition 범위로 매핑
        float newZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 247280.0f);

        // 부모 객체의 Z 위치를 이동 속도에 따라 일정하게 이동
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(currentPosition, new Vector3(currentPosition.x, currentPosition.y, newZPosition), moveSpeed * Time.deltaTime);

        // 자식 객체 이동 (부모 이동의 절반만큼 Z축으로 이동)
        if (childObject != null)
        {
            // 부모의 Z 이동에 따라 자식의 Z 위치 계산
            float childZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 100.0f) * 0.5f;
            Vector3 currentChildPosition = childObject.position;
            childObject.position = Vector3.MoveTowards(currentChildPosition, new Vector3(currentChildPosition.x, currentChildPosition.y, childZPosition), moveSpeed * Time.deltaTime * 0.5f);
        }
    }


        void writeMyPos()
    {
        intinputvalue = (int)inputValue;
        systemManager.WriteModbusRegisters(201, intinputvalue);

        // 입력 값을 0과 100 사이로 제한
        inputValue = Mathf.Clamp(inputValue, 0.0f, 247280.0f);

        // 입력 값을 0~100 범위에서 minZPosition~maxZPosition 범위로 매핑
        float newZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 247280.0f);

        // 부모 객체의 Z 위치를 이동 속도에 따라 일정하게 이동
        Vector3 currentPosition = transform.position;
        transform.position = Vector3.MoveTowards(currentPosition, new Vector3(currentPosition.x, currentPosition.y, newZPosition), moveSpeed * Time.deltaTime);

        // 자식 객체 이동 (부모 이동의 절반만큼 Z축으로 이동)
        if (childObject != null)
        {
            // 부모의 Z 이동에 따라 자식의 Z 위치 계산
            float childZPosition = Mathf.Lerp(minZPosition, maxZPosition, inputValue / 100.0f) * 0.5f;
            Vector3 currentChildPosition = childObject.position;
            childObject.position = Vector3.MoveTowards(currentChildPosition, new Vector3(currentChildPosition.x, currentChildPosition.y, childZPosition), moveSpeed * Time.deltaTime * 0.5f);
        }
    }
}
