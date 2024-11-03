using UnityEngine;

public class ParentChildOscillateY : MonoBehaviour
{

    private SystemManager systemManager;

    private int intinputvalue;

    public Transform childObject; // 자식 객체

    // 0에서 100 사이의 입력 값
    public float inputValue = 0.0f;

    // 부모 객체가 이동할 수 있는 Y 좌표의 최소값과 최대값
    public float minYPosition = 900.0f;
    public float maxYPosition = 1125.0f;

    // 이동 속도 설정 (기본값 10)
    private float moveSpeed = 1f;

    void Start()
    {
        
        systemManager = FindObjectOfType<SystemManager>();

        if (systemManager == null)
        {
            Debug.LogError("SystemManager not found in the scene.");
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
                inputValue = 4350;
                break;
            case 3:
                // Clamp up Position
                inputValue = 20000;
                break;
            case int n when (n >= 20 && n <= 28):
                // Storage input position에서 20~28 처리
                inputValue = Basket_Controller.coordination[n % 10, 1];
                break;
            case int n when (n >= 30 && n <= 38):
                // Storage Unclamp position에서 30~38 처리
                inputValue = Basket_Controller.coordination[n % 10, 1]-1000;
                break;
            default:
                // 위의 값들과 일치하지 않는 경우
                break;
        }
    



        // inputValue를 0과 100 사이로 제한
        inputValue = Mathf.Clamp(inputValue, 0.0f, 20000f);

        // inputValue를 951과 1149 사이의 Y 위치로 매핑
        float newYPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 20000f);

        // 부모 객체의 Y 위치 설정
        Vector3 parentPosition = transform.position;
        parentPosition.y = newYPosition;
        transform.position = Vector3.Lerp(transform.position, parentPosition, moveSpeed * Time.deltaTime);




        // 자식 객체 이동 (부모의 이동의 절반만큼 Z 축으로 이동)
        if (childObject != null)
        {
            // 자식 객체의 Z 위치 계산
            float childZPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 100.0f) * 0.5f;

            // 자식 객체의 Z 위치 설정 (X와 Y는 유지)
            Vector3 childPosition = childObject.position;
            childPosition.z = childZPosition;
            childObject.position = childPosition;
        }
    }


    private void writeMyPos() 
    {
        intinputvalue = (int)inputValue;
        systemManager.WriteModbusRegisters(202, intinputvalue);

        // inputValue를 0과 100 사이로 제한
        inputValue = Mathf.Clamp(inputValue, 0.0f, 20000f);

        // inputValue를 951과 1149 사이의 Y 위치로 매핑
        float newYPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 20000f);

        // 부모 객체의 Y 위치 설정
        Vector3 parentPosition = transform.position;
        parentPosition.y = newYPosition;
        transform.position = Vector3.Lerp(transform.position, parentPosition, moveSpeed * Time.deltaTime);




        // 자식 객체 이동 (부모의 이동의 절반만큼 Z 축으로 이동)
        if (childObject != null)
        {
            // 자식 객체의 Z 위치 계산
            float childZPosition = Mathf.Lerp(minYPosition, maxYPosition, inputValue / 100.0f) * 0.5f;

            // 자식 객체의 Z 위치 설정 (X와 Y는 유지)
            Vector3 childPosition = childObject.position;
            childPosition.z = childZPosition;
            childObject.position = childPosition;
        }
    }
}
