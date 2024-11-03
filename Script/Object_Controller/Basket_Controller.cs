using System.Collections;
using System.Runtime.Serialization;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Basket_Controller : MonoBehaviour
{
    public GameObject obj_stopper0;
    public GameObject obj_stopper1;
    public GameObject obj_stopper2;
    public GameObject obj_GantryX;
    public GameObject obj_GantryY;
    public GameObject obj_Gantry_ROT;
    public GameObject obj_Gantry_FB;
    public GameObject obj_Gantry_CLP;
    public GameObject obj_Gantry_OUT;

    private SystemManager systemManager;

    private bool isGrabbed = false;
    private bool hasGrabbed= false;

    public Vector3[] points; // 이동할 좌표들
    private float speed = 150f; // 이동 속도

    private int currentPointIndex = 0; // 현재 목표 포인트 인덱스
    private bool hasReachedEnd = false; // 모든 포인트에 도달했는지 여부
    private BoxCollider boxCollider; // 활성화할 BoxCollider

    public GameObject prefabToSpawn; // 생성할 프리팹을 위한 변수
    private Rigidbody Rigid;

    public float pushForce = 5.0f; // 충돌 시 물체에 가할 힘

    private bool isPaused = false; // 일시 정지 상태 확인 변수


    private int containlocate = 0;

    Vector3 targetDisposeXPosition1 = new Vector3(0, 0, 340);
    Vector3 targetDisposeYPosition2 = new Vector3(120, -40, 341);



    private bool[] simulation_contains = new bool[9];
    public static int[,] coordination = new int[,]
    {
        { 240302,19526 },  // 첫 번째 행
        { 127946, 19526 },   // 두 번째 행
        { 330, 19526 },
        { 240302, 10851 },
        { 127946, 10851 },
        { 330, 10851 },
        { 240302, 1860 },
        { 127946, 1860 },
        { 330, 1860 },

    };


    

    void Start()
    {
        Debug.Log("안녕사에ㅛ");
        obj_stopper0 = GameObject.Find("Stopper_0-1");
        obj_stopper1 = GameObject.Find("Stopper_1-1");
        obj_stopper2 = GameObject.Find("Stopper_2-1");
        obj_GantryX = GameObject.Find("Xaxis_moving_object");
        obj_GantryY = GameObject.Find("Yaxis_moving_object");
        obj_Gantry_ROT = GameObject.Find("Gantry_rotator");
        obj_Gantry_FB = GameObject.Find("Gantry_Forward/Backward");
        obj_Gantry_CLP = GameObject.Find("Gantry_gripper");
        obj_Gantry_OUT = GameObject.Find("Out_Cylinder");


        systemManager = FindObjectOfType<SystemManager>();
        // 자기 자신의 BoxCollider를 찾음
        boxCollider = GetComponent<BoxCollider>();
        Rigid = GetComponent<Rigidbody>();


        for (int i = 0; i < simulation_contains.Length; i++)
        {
            simulation_contains[i] = false;
        }

        systemManager.WriteModbusCoil(6960, true);

    }

    void Update()
    {
        if (isPaused) return; // 일시 정지 중일 때는 Update 실행 안함

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
       
        Debug.Log("그리퍼그리퍼그리퍼 : " + systemManager.ReadModbusCoil(1109, 1));
        if (systemManager.ReadModbusCoil(1109, 1) && !hasGrabbed)
        {
            this.transform.SetParent(obj_Gantry_CLP.transform);// 잡을 물체를 그리퍼에 고정
            isGrabbed = true; // 잡힌 상태로 변경
        }
        else 
        {
            Vector3 originalPosition = this.transform.position;
            Quaternion originalRotation = this.transform.rotation;

            // 부모 관계 해제
            this.transform.SetParent(null);

            // 부모 관계 해제 후 원래 위치와 회전 값 적용
            this.transform.position = originalPosition;
            this.transform.rotation = originalRotation;
            if(isGrabbed) hasGrabbed = true;
            
        }
            



        if (hasReachedEnd || points.Length == 0)
            return;

        // 현재 목표 좌표로 이동
        Vector3 targetPoint = points[currentPointIndex];
        Vector3 direction = targetPoint - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // 물체를 목표 좌표 방향으로 이동
        if (direction.magnitude <= distanceThisFrame)
        {
            transform.position = targetPoint; // 목표 좌표에 도달

            if (currentPointIndex == 0)
            {
                if (!isPaused)
                {
                    StartCoroutine(PauseAndWaitForModbusCoil()); // 코루틴 시작
                }
            }
            else
            {
                currentPointIndex++;
            }

            // 모든 포인트를 순회했는지 확인
            if (currentPointIndex >= points.Length)
            {
                obj_Gantry_OUT.GetComponent<Out_Cylinder_Controller>().currentValue = false;
                hasReachedEnd = true;
            }
        }
        else
        {
            transform.position += direction.normalized * distanceThisFrame; // 이동
        }

    }

    void RunSimulation()
    {
        if (hasReachedEnd || points.Length == 0)
            return;
            
        // 현재 목표 좌표로 이동
        Vector3 targetPoint = points[currentPointIndex];
        Vector3 direction = targetPoint - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        // 물체를 목표 좌표 방향으로 이동
        if (direction.magnitude <= distanceThisFrame)
        {
            transform.position = targetPoint; // 목표 좌표에 도달


            if (currentPointIndex == 0)
            {
                StartCoroutine(Pause1sec());

            }
            else if(currentPointIndex == 1)
            {
                obj_Gantry_OUT.GetComponent<Out_Cylinder_Controller>().currentValue = true;
                currentPointIndex++;
            }
            else
            {
                currentPointIndex++;
            }

            // 모든 포인트를 순회했는지 확인
            if (currentPointIndex >= points.Length)
            {
                obj_Gantry_OUT.GetComponent<Out_Cylinder_Controller>().currentValue = false;
                hasReachedEnd = true;

                StartCoroutine(IncomingSequence(systemManager.incomingitem));
                systemManager.incomingitem = 0;
            }
        }
        else
        {
            transform.position += direction.normalized * distanceThisFrame; // 이동
        }
    }

    void RunManualControl()
    {
        //메뉴얼모드에서는 X
    }



    IEnumerator PauseAndWaitForModbusCoil()
    {
        isPaused = true;

        // 6960번 coil이 true인지 계속 확인
        while (systemManager.ReadModbusCoil(1103, 1))
        {
            yield return null; // 매 프레임마다 확인
        }

        // 6960번 coil이 false가 되면 이동 재개
        isPaused = false;
        currentPointIndex++;
    }





    IEnumerator IncomingSequence(int item)
    {
        systemManager.ismoving = true;


        // 클램프 업포지션으로 이동
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 95167;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        // 회전
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = true;

        
        yield return new WaitForSeconds(3f);// 3초 대기

        //클램프 포지션으로 이동
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 4350;

        yield return new WaitForSeconds(6f); // 5초대기

        // 전진 & 그립
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;
        yield return new WaitForSeconds(2f);// 3초 대기
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = true;
        
        this.transform.SetParent(obj_Gantry_CLP.transform);// 잡을 물체를 그리퍼에 고정


        yield return new WaitForSeconds(1f); //1초대기 

        //// 클램프 업포지션으로 이동
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        // 후진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        // 0도로 회전
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = false;

        yield return new WaitForSeconds(2f);


        // 빈 스토리지로 이동
        for (containlocate = 0; containlocate < systemManager.simulation_contains.Length; containlocate++)
        {

            if(systemManager.simulation_contains[containlocate] == 0)
            {
                obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = coordination[containlocate, 0];
                obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = coordination[containlocate, 1];
                Debug.Log($"coordination[containlocate, 0]: {coordination[containlocate, 0]}");
                Debug.Log($"coordination[containlocate, 1]: {coordination[containlocate, 1]}");

                systemManager.simulation_contains[containlocate] = item;
                this.gameObject.name = "box_" + containlocate;
                break;
            }
            if (containlocate == systemManager.simulation_contains.Length)
                Debug.LogError("컨테이너 꽉참");
        }

        yield return new WaitForSeconds(3f);

        //전진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;


        yield return new WaitForSeconds(1f);// 1초 대기

        //놓기
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = true;

        Vector3 originalPosition = this.transform.position;
        Quaternion originalRotation = this.transform.rotation;

        // 부모 관계 해제
        this.transform.SetParent(null);

        // 부모 관계 해제 후 원래 위치와 회전 값 적용
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        yield return new WaitForSeconds(1f);

        //후진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        yield return new WaitForSeconds(1f);


        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 0;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 0;
        yield return new WaitForSeconds(1f);

        systemManager.ismoving= false;

    }

    public IEnumerator OutgoingSequence()
    {
        systemManager.ismoving = true;
        // 겐트리 회전
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = false;

        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = coordination[containlocate, 0];
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = coordination[containlocate, 1] - 20;

        
        systemManager.simulation_contains[containlocate] = 0;

        yield return new WaitForSeconds(6f);
        


        //전진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;

        yield return new WaitForSeconds(2f);

        // 집는 동작 고정
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = true;

        yield return new WaitForSeconds(1f);

        // 잡을 물체를 그리퍼에 고정
        this.transform.SetParent(obj_Gantry_CLP.transform);


        // 후진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        yield return new WaitForSeconds(1f);



        // 클램프 업포지션으로 이동
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 95167;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        yield return new WaitForSeconds(1f);

        // 회전
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = true;

        

        //전진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = true;

        // 클램프 포지션으로 이동
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 4350;
        yield return new WaitForSeconds(6f);


        //놓기
        obj_Gantry_CLP.GetComponent<Gantry_Gripper_Controller>().Grip = false;


        Vector3 originalPosition = this.transform.position;
        Quaternion originalRotation = this.transform.rotation;

        // 부모 관계 해제
        this.transform.SetParent(null);

        // 부모 관계 해제 후 원래 위치와 회전 값 적용
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;

        yield return new WaitForSeconds(1f);

        //후진
        obj_Gantry_FB.GetComponent<Gantry_ForwardBackward_Controller>().moveForward = false;

        yield return new WaitForSeconds(1f);


        // 클램프 업포지션으로 이동
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 20000;

        yield return new WaitForSeconds(3f);
        //로터리 0으로 이동
        obj_Gantry_ROT.GetComponent<ConditionalRotateObject>().currentValue = false;

        //원점 이동
        obj_GantryX.GetComponent<ParentChildOscillateZ>().inputValue = 0;
        obj_GantryY.GetComponent<ParentChildOscillateY>().inputValue = 0;


        obj_stopper2.GetComponent<Stopper2_Controller>().moveforward = false;



        StartCoroutine(MoveToPositionsSequentially());

        yield return new WaitForSeconds(3f);
        obj_stopper2.GetComponent<Stopper2_Controller>().moveforward = true;
        systemManager.ismoving = false;

        

    }

    // 첫 번째 포인트에서 1초 동안 일시 정지하는 코루틴
    IEnumerator Pause1sec()
    {
        obj_stopper0.GetComponent<Stopper0_Controller>().currentValue = true;
        isPaused = true; // 이동 중지
        yield return new WaitForSeconds(2f); // 2초 대기
        obj_stopper1.GetComponent<Stopper1_Controller>().currentValue = false;
        yield return new WaitForSeconds(1f); // 1초 대기
        obj_stopper0.GetComponent<Stopper0_Controller>().currentValue = false;
        currentPointIndex++; // 다음 포인트로 이동
        isPaused = false; // 이동 재개
        yield return new WaitForSeconds(2f); // 3초 대기
        obj_stopper1.GetComponent<Stopper1_Controller>().currentValue = true;

    }

    IEnumerator MoveToPositionsSequentially()
    {
        // 첫 번째 좌표로 이동
        yield return StartCoroutine(MoveToPosition(targetDisposeXPosition1));

        // 두 번째 좌표로 이동
        yield return StartCoroutine(MoveToPosition(targetDisposeYPosition2));


        yield return new WaitForSeconds(2f);
        

        Destroy(this.gameObject);


    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f) // 목표 위치에 가까워질 때까지 반복
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null; // 한 프레임 대기
        }
        transform.position = targetPosition; // 최종적으로 정확한 목표 위치에 도달
    }

}
