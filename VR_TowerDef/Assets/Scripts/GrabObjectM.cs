using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectM : MonoBehaviour
{
    // 필요 속성: 물체를 잡고 있는지 여부, 잡고 있는 물체, 잡을 물체의 종류, 잡을 수 있는 거리
    // 물체를 잡고 있는지 여부
    private bool isGragbbing = false;
    // 잡고 있는 물체
    private GameObject grabbedObj;
    // 잡을 물체의 종류
    public LayerMask grabableLayer;
    // 잡을 수 있는 거리
    public float grabRange = 0.2f;

    // { 물체를 던지기 위한 변수
    // 이전 위치
    private Vector3 prevPos;
    // 던질 힘
    private float throwPower = 10f;

    // 이전 회전
    private Quaternion prevRot;
    // 회전력
    public float rotPower = 5f;

    // 원거리에서 물체를 잡는 기능 활성화 여부
    public bool isRemoteGrab = true;
    // 원거리에서 물체를 잡을 수 있는 거리
    public float remoteGrabDis = 20f;
    // } 물체를 던지기 위한 변수

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 물체 잡기
        // 1. 물체를 잡지 않고 있을 경우
        if (isGragbbing == false)
        {
            // 잡기 시도
            TryGrab();
        }
        else
        {
            // 물체 놓기
            TryUnGrab();
        }
    }

    // 물체를 잡는 함수
    void TryGrab()
    {
        // [Grab] 버튼을 누르면 일정 영역 안에 있는 폭탄을 잡는다.
        // 1. [Grab] 버튼을 눌렀다면
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 원거리 물체 잡기를 사용한다면
            if(isRemoteGrab)
            {
                // 손 방향으로 레이 제작
                Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
                RaycastHit hitInfo;
                // SphereCast를 이용해 물체 충돌을 체크
                if(Physics.SphereCast(ray, 0.5f, out hitInfo, remoteGrabDis, grabableLayer))
                {
                    // 잡은 상태로 전환
                    isGragbbing = true;
                    // 잡은 물체를 캐싱
                    grabbedObj = hitInfo.transform.gameObject;
                    // 물체가 끌려오는 기능 실행
                    StartCoroutine(GrabbingAnimation());
                }
                return;
            }               // if(isRemoteGrab)

            // 2. 일정 영역 안에 폭탄이 있을 때
            // 영역 안에 있는 모든 폭탄 검출
            Collider[] hitObjs = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabableLayer);
            // 가장 가까운 폭탄 인덱스
            int closest = 0;
            // 손과 가장 가까운 물체 선택
            for (int i = 1; i < hitObjs.Length; i++)
            {
                // 손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObjs[closest].transform.position;
                float closestDis = Vector3.Distance(ARAVRInput.RHandPosition, closestPos);
                // 다음 물체와 손의 거리
                Vector3 nextPos = hitObjs[i].transform.position;
                float nextDis = Vector3.Distance(ARAVRInput.RHandPosition, nextPos);
                // 다음 물체와의 거리가 더 가깝다면
                if (nextDis < closestDis)
                {
                    // 가장 가까운 물체 인덱스 교체
                    closest = i;
                }
            }

            // 3. 폭탄을 잡는다.
            // 검출된 물체가 있을 경우
            if (hitObjs.Length > 0)
            {
                // 잡은 상태로 전환
                isGragbbing = true;
                // 잡은 물체를 캐싱
                grabbedObj = hitObjs[closest].gameObject;
                // 잡은 물체를 손의 자식으로 등록
                // grabbedObj.transform.parent = ARAVRInput.RHand;
                grabbedObj.transform.SetParent(ARAVRInput.RHand, true);

                // 초기 위치 값 지정
                prevPos = ARAVRInput.RHandPosition;
                // 초기 회전 값 지정
                prevRot = ARAVRInput.RHand.rotation;
            }

            // 물리 기능 정지
            grabbedObj.GetComponent<Rigidbody>().isKinematic = true;
        }
    }               // TryGrab()

    // 물체를 내려놓는 함수
    void TryUnGrab()
    {
        // 던질 방향
        Vector3 throwDir = (ARAVRInput.RHandPosition - prevPos);
        // 이전 위치 갱신
        prevPos = ARAVRInput.RHandPosition;

        /* 
         * 쿼터니온 공식
         * angle1 = Q1, angle2 = Q2
         * angle1 + angle2 = Q1 * Q2
         * -angle2 = Quaternion.Inverse(Q2)
         * angle2 - angle1 = Quaternion.FromToRotation(Q1,Q2) = Q2 * Quaternion.Inverse(Q1);
         */

        // 회전방향 = current - previous의 차로 구함. -previous는 Inverse로 구함(무지성으로 Quaternion 앞에 - 붙인다고 되는게 아님)
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRot);
        // 이전 회전을 캐싱한 변수를 갱신
        prevRot = ARAVRInput.RHand.rotation;

        // 버튼을 놓았다면
        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 잡지 않은 상태로 전환
            isGragbbing = false;
            // 물리 기능 활성화
            grabbedObj.GetComponent<Rigidbody>().isKinematic = false;
            // 손에서 폭탄 떼어내기
            // grabbedObj.transform.parent = null;
            grabbedObj.transform.SetParent(default, true);
            // 던지기
            grabbedObj.GetComponent<Rigidbody>().velocity = throwDir * throwPower;

            // 각속도 = (1/dt) * dθ(특정 축 기준 변위 각도)
            float angle = default;
            Vector3 axis = default;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbedObj.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

            // 잡은 물체가 없도록 설정
            grabbedObj = null;
        }
    }               // TryUnGrab()

    // 물체 끌어오는 기능 함수
    private IEnumerator GrabbingAnimation()
    {
        // 물리 기능 정지
        grabbedObj.GetComponent<Rigidbody>().isKinematic = true;
        // 초기 위치 값 지정
        prevPos = ARAVRInput.RHandPosition;
        // 초기 회전 값 지정
        prevRot = ARAVRInput.RHand.rotation;
        Vector3 startLocation = grabbedObj.transform.position;
        Vector3 targetLocation = ARAVRInput.RHandPosition + (ARAVRInput.RHandDirection * 0.1f);

        float currentTime = 0f;
        float finishTime = 0.2f;
        // 경과율
        float elapsedRate = currentTime / finishTime;
        while(elapsedRate < 1)
        {
            currentTime += Time.deltaTime;
            elapsedRate = currentTime / finishTime;
            grabbedObj.transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate);
            yield return null;
        }

        // 잡은 물체를 손의 자식으로 등록
        grabbedObj.transform.position = targetLocation;
        grabbedObj.transform.parent = ARAVRInput.RHand;
    }
}
