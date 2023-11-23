using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GazePointerCtrl : MonoBehaviour
{
    public Video360Play video360;

    public Transform uiCanvas;
    public Image gazeImg;

    Vector3 defaultScale;
    public float uiScaleVal = 1f;

    private bool isHitObj = false;
    private GameObject prevHitObj = default;
    private GameObject currentHitObj = default;
    float currentGazeTime = 0;
    public float gazeChargeTime = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        defaultScale = uiCanvas.localScale;
        currentGazeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 캔버스 오브젝트의 스케일을 거리에 따라 조절한다
        // 1. 카메라를 기준으로 전방 방향의 좌표를 구한다
        Vector3 dir = transform.TransformPoint(Vector3.forward);
        // 2. 카메라를 기준으로 전방의 레이를 설정한다.
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo; // 히트된 오브젝트의 정보를 캐싱한다
        // 3. 레이에 부딪힌 경우에는 거리 값을 이용해 uiCanvas의 크기를 조절한다.
        if (Physics.Raycast(ray, out hitInfo))
        {
            uiCanvas.localScale = defaultScale * uiScaleVal * hitInfo.distance;
            uiCanvas.position = transform.forward * hitInfo.distance;
            if(hitInfo.transform.tag == "GazeObj")
            {
                isHitObj = true;
            }
            currentHitObj = hitInfo.transform.gameObject;
        }
        else // 4. 아무것도 부딪히지 않으면 기본 스케일 값으로 uiCanvas의 크기를 조절한다.
        {
            uiCanvas.localScale = defaultScale * uiScaleVal;
            uiCanvas.position = transform.position + dir;
        }
        // 5. uiCanvas가 항상 카메라 오브젝트를 바라보게 한다
        uiCanvas.forward = transform.forward * -1;

        // GazeObj에 레이가 닿았을 때 실행
        if(isHitObj)
        {
            if(currentHitObj == prevHitObj)     // 현재 프레임과 이전 프레임의 게임 오브젝트가 같아야 시간 증가
            {
                // 인터랙션이 발생해야 하는 오브젝트에 시선이 고정돼 있다면 시간 증가
                currentGazeTime += Time.deltaTime;
            }
            else                                // 현재 프레임의 오브젝트가 이전 프레임의 오브젝트에서 벗어난 경우
            {
                // 이전 프레임의 영상 정보를 업데이트한다.
                prevHitObj = currentHitObj;
            }
            // hit된 오브젝트가 VideoPlayer 컴포넌트를 갖고 있는지 확인한다
            HitObjChecker(currentHitObj, true);
        }
        else        // 시선이 벗어났거나 GazeObj가 아닌 오브젝트를 바라보는 경우
        {
            if(prevHitObj != null)
            {
                HitObjChecker(prevHitObj, false);
                prevHitObj = null;  // prevHitObj의 정보를 지운다
            }
            currentGazeTime = 0;
        }
        // 시선이 머문 시간을 0과 최댓값 사이로 한다
        currentGazeTime = Mathf.Clamp(currentGazeTime, 0, gazeChargeTime);
        // ui Image의 fillAmount를 업데이트한다.
        gazeImg.fillAmount = currentGazeTime / gazeChargeTime;

        // gazePointer의 게이지를 한 프레임 만큼 올린 다음에 현재 프레임에 사용된 변수들을 초기화한다.
        isHitObj = false;       // 모든 처리가 끝나면 isHitObj를 false로 한다.
        currentHitObj = null;   // currentHitObj의 정보를 지운다.
    }

    // 히트된 오브젝트 타입별로 작동 방식을 구분한다.
    void HitObjChecker(GameObject hitObj, bool isActive)
    {
        // hit가 비디오 플레이어 컴포넌트를 갖고 있는지 확인한다.
        if(hitObj.GetComponent<VideoPlayer>())
        {
            if(isActive)
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(true);
            }
            else
            {
                hitObj.GetComponent<VideoFrame>().CheckVideoFrame(false);
            }
        }
        
        if(currentGazeTime / gazeChargeTime >= 1)
        {
            if(hitObj.name.Contains("Right"))
            {
                video360.SwapVideoClip(true);
            }
            else if (hitObj.name.Contains("Left"))
            {
                video360.SwapVideoClip(false);
            }
            else
            {
                video360.SetVideoPlay(currentHitObj.transform.GetSiblingIndex());
            }
            currentGazeTime = 0;
        }
    }
}
