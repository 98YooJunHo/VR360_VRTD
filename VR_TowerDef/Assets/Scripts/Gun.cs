using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // { 총알 관련 변수
    public Transform bulletImpact = default;
    private ParticleSystem bulletEffect = default;
    private AudioSource bulletAudio = default;
    // } 총알 관련 변수

    // { 조준점 관련 변수
    public Transform crosshair = default;
    // { 조준점 관련 변수

    // Start is called before the first frame update
    void Start()
    {
        // 총알 Effect = FX 파티클 시스템 컴포넌트 가져오기
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        // 총알 효과 오디오 소스 컴포넌트 가져오기
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // 크로스헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);

        // 사용자가 IndexTrigger 버튼을 누르면
        if(ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // 컨트롤러의 진동 재생
            ARAVRInput.PlayVibration(0.6f, 1f, 1f, ARAVRInput.Controller.RTouch);

            // { 총알 오디오 재생
            bulletAudio.Stop();
            bulletAudio.Play();
            // } 총알 오디오 재생

            // Ray를 카메라의 위치로부터 나가도록 만든다.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            // Ray의 충돌 정보를 저장하기 위한 변수 지정
            RaycastHit hitInfo;
            // 플레이어 레이어 얻어오기 (<<는 비트를 왼쪽으로 옮기는 연산임 
            // player레이어의 경우 9번 레이어 이므로 9번 왼쪽으로 이동 즉 2의 9제곱)
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            // 타워 레이어 얻어오기 (타워의 경우 8번 레이어 이므로 2의 8제곱)
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMask = playerLayer | towerLayer;
            // Ray를 쏘고, 부딪힌 정보는 hitInfo에 담는다 
            // (~ 역(반대) 이 아래의 식은 설정한 범위 외의 것을 탐지하기 위함)
            if(Physics.Raycast(ray, out hitInfo, 200f, ~layerMask))
            {
                // 총알 파편 효과 처리
                // 총알 이펙트 진행되고 있으면 멈추고 재생
                bulletEffect.Stop();
                bulletEffect.Play();
                // 부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
                bulletImpact.position = hitInfo.point;
                // 부딪힌 지점의 방향으로 총알 이펙트의 방향을 설정
                bulletImpact.forward = hitInfo.normal;
            }
        }
    }
}
