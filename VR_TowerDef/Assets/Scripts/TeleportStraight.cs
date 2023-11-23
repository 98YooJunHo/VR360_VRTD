using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TeleportStraight : MonoBehaviour
{
    // 텔레포트를 표시할 UI
    public Transform teleportCircleUI = default;
    // 선을 그릴 라인 렌더러
    private LineRenderer lineRenderer = default;
    // 최초 텔레포트 UI의 크기
    Vector3 originScale = Vector3.one * 0.02f;

    // { 워프에 사용할 변수
    // 워프 사용 여부
    public bool isWarp = false;
    // 워프에 걸리는 시간
    public float warpTime = 0.1f;
    // 사용하고 있는 포스트 프로세싱 볼륨 컴포넌트
    public PostProcessVolume psVolume = default;
    // } 워프에 사용할 변수

    private void Awake()
    {
        // 시작할 때 비활성화한다.
        teleportCircleUI.gameObject.SetActive(false);
        // 라인 렌더러 컴포넌트 얻어오기
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 왼쪽 컨트롤러의 One 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 활성화
            lineRenderer.enabled = true;
        }

        // 왼쪽 컨트롤러의 One 버튼에서 손을 떼면
        if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 라인 렌더러 컴포넌트 비활성화
            lineRenderer.enabled = false;

            if(teleportCircleUI.gameObject.activeSelf)
            {
                // 워프 기능이 사용이 아닐 때 순간이동 처리
                if(isWarp == false)
                {
                    GetComponent<CharacterController>().enabled = false;
                    // 텔레포트 UI 위치로 순간 이동
                    transform.position = teleportCircleUI.position + Vector3.up;
                    GetComponent<CharacterController>().enabled = true;
                }
                else
                {
                    // 워프 기능을 사용할 때는 Warp() 코루틴 호출
                    StartCoroutine(Warp());
                }
            }

            // 텔레포트 UI 비활성화
            teleportCircleUI.gameObject.SetActive(false);
        }

        // 왼쪽 컨트롤러의 One 버튼을 누르고 있을 때
        if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            // 1.왼쪽 컨트롤러를 기준으로 Ray를 만든다
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection);
            RaycastHit hitInfo;
            int layer = 1 << LayerMask.NameToLayer("Terrain");

            // 2.Terrain만 Ray 충돌 검출
            if (Physics.Raycast(ray, out hitInfo, 200f, layer))
            {
                // 3.Ray가 부딪힌 지점에 라인 그리기 (시작점 레이 시작점, 끝나는 지점 부딪힌 지점)
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, hitInfo.point);
                // 4.Ray가 부딪힌 지점에 텔레포트 UI 표시
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = hitInfo.point;
                // 텔레포트 UI가 위로 누워 있도록 방향 설정
                teleportCircleUI.forward = hitInfo.normal;
                // 텔레포트 UI의 크기가 거리에 따라 보정되도록 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1f, hitInfo.distance);
            }
            else
            {
                // Ray 충돌이 발생하지 않으면 선이 Ray 방향으로 그려지도록 처리
                lineRenderer.SetPosition(0, ray.origin);
                lineRenderer.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200f);
                // 텔레포트 UI는 화면에서 비활성화
                teleportCircleUI.gameObject.SetActive(false);
            }
        }
    }

    //! 워프 효과를 내는 코루틴
    IEnumerator Warp()
    {
        // 워프 느낌을 표현할 모션블러
        MotionBlur mBlur;
        // 워프 시작점 기억
        Vector3 pos = transform.position;
        // 목적지
        Vector3 targetPos = teleportCircleUI.position + Vector3.up;
        // 워프 경과 시간
        float currentTime = 0f;
        // 포스트 포르세싱에서 사용 중인 프로파일에서 모션블러 얻어오기
        psVolume.profile.TryGetSettings<MotionBlur>(out mBlur);
        // 워프 시작전 블러 켜기
        mBlur.active = true;
        GetComponent<CharacterController>().enabled = false;

        // 경과 시간이 워프보다 짧은 시간 동안 이동 처리
        while(currentTime < warpTime)
        {
            // 경과 시간 흐르게 하기
            currentTime += Time.deltaTime;
            // 워프 시작 점에서 도착점에 도착하기 위해 워프 시간 동안 이동
            transform.position = Vector3.Lerp(pos, targetPos, currentTime / warpTime);
            // 코루틴 대기
            yield return null;
        }

        // 텔레포트 UI 위치로 순간이동
        transform.position = targetPos;
        // 캐릭터 컨트롤러 다시 켜기
        GetComponent<CharacterController>().enabled = true;
        // 포스트 효과 끄기
        mBlur.active = false;
    }
}
