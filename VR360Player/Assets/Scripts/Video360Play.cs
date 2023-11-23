using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video360Play : MonoBehaviour
{
    VideoPlayer videoPlayer;
    // 재생해야 할 vr 360 영상을 위한 설정
    public VideoClip[] vcList = default;
    int currentVcIdx = default;

    // Start is called before the first frame update
    void Start()
    {
        // 비디오 플레이어 컴포넌트의 정보를 받아온다
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.clip = vcList[0];
        currentVcIdx = 0;
        videoPlayer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // 컴퓨터에서 영상을 변경하기 위한 기능
        if (Input.GetKeyDown(KeyCode.LeftBracket))          // 왼쪽 대괄호 입력 시 이전 영상
        {
            SwapVideoClip(false);
            // videoPlayer.clip = vcList[0];
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))    // 오른쪽 대괄호 입력 시 다음 영상
        {
            SwapVideoClip(true);
            // videoPlayer.clip = vcList[1];
        }
    }

    /**
     * 인터랙션을 위해 함수를 퍼블릭으로 선언한다
     * @brief 배열의 인덱스 번호를 기준으로 영상을 교체, 재생하기 위한 함수
     * @param isNext가 true이면 다음 영상, false면 이전 영상을 재생
     */
    public void SwapVideoClip(bool isNext)
    {
        /**
         * 현재 재생 중인 영상의 넘버를 기준으로 체크한다
         * 이전 영상 번호는 현재 영상보다 배열에서 인덱스 번호가 1이 작다.
         * 다음 영상 번호는 현재 영상보다 배열에서 인덱스 번호가 1이 크다.
         */
        int setVCnum = currentVcIdx;    // 현재 재생 중인 영상의 넘버를 입력한다.
        videoPlayer.Stop();             // 현재 재생 중인 비디오 클립을 중지한다.
        // 재생될 영상을 고르기 위한 과정
        if(isNext)
        {
            // 리스트 전체 길이보다 크면 클립을 리스트의 첫 번째 영상으로 지정한다.
            setVCnum = (setVCnum + 1) % vcList.Length;

            //// 배열의 다음 영상을 재생한다
            //setVCnum++;
            //if(setVCnum >= vcList.Length)
            //{
            //    // 리스트 전체 길이보다 크면 리스트의 클립을 첫 번째 영상으로 지정한다.
            //    videoPlayer.clip = vcList[0];
            //}
            //else
            //{
            //    // 리스트 길이보다 작으면 해당 번호의 영상을 실행한다.
            //    videoPlayer.clip = vcList[setVCnum];
            //}
        }
        else
        {
            // 배열의 이전 영상을 재생한다
            setVCnum =  ((setVCnum - 1) + vcList.Length) % vcList.Length;
        }
        videoPlayer.clip = vcList[setVCnum];    // 클립을 변경한다.
        videoPlayer.Play();                     // 바뀐 클립을 재생한다.
        currentVcIdx = setVCnum;                // 바뀐 클립의 영상의 번호를 업데이트한다.
    }

    public void SetVideoPlay(int num)
    {
        // 현재 재생중인번호가 전달받은 번호와 다를 때만 실행
        if(num != currentVcIdx)
        {
            videoPlayer.Stop();
            videoPlayer.clip = vcList[num];
            currentVcIdx = num;
            videoPlayer.Play();
        }
    }
}
