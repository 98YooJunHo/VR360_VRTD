using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video360Play : MonoBehaviour
{
    VideoPlayer videoPlayer;
    // ����ؾ� �� vr 360 ������ ���� ����
    public VideoClip[] vcList = default;
    int currentVcIdx = default;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �÷��̾� ������Ʈ�� ������ �޾ƿ´�
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.clip = vcList[0];
        currentVcIdx = 0;
        videoPlayer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // ��ǻ�Ϳ��� ������ �����ϱ� ���� ���
        if (Input.GetKeyDown(KeyCode.LeftBracket))          // ���� ���ȣ �Է� �� ���� ����
        {
            SwapVideoClip(false);
            // videoPlayer.clip = vcList[0];
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))    // ������ ���ȣ �Է� �� ���� ����
        {
            SwapVideoClip(true);
            // videoPlayer.clip = vcList[1];
        }
    }

    /**
     * ���ͷ����� ���� �Լ��� �ۺ����� �����Ѵ�
     * @brief �迭�� �ε��� ��ȣ�� �������� ������ ��ü, ����ϱ� ���� �Լ�
     * @param isNext�� true�̸� ���� ����, false�� ���� ������ ���
     */
    public void SwapVideoClip(bool isNext)
    {
        /**
         * ���� ��� ���� ������ �ѹ��� �������� üũ�Ѵ�
         * ���� ���� ��ȣ�� ���� ���󺸴� �迭���� �ε��� ��ȣ�� 1�� �۴�.
         * ���� ���� ��ȣ�� ���� ���󺸴� �迭���� �ε��� ��ȣ�� 1�� ũ��.
         */
        int setVCnum = currentVcIdx;    // ���� ��� ���� ������ �ѹ��� �Է��Ѵ�.
        videoPlayer.Stop();             // ���� ��� ���� ���� Ŭ���� �����Ѵ�.
        // ����� ������ ���� ���� ����
        if(isNext)
        {
            // ����Ʈ ��ü ���̺��� ũ�� Ŭ���� ����Ʈ�� ù ��° �������� �����Ѵ�.
            setVCnum = (setVCnum + 1) % vcList.Length;

            //// �迭�� ���� ������ ����Ѵ�
            //setVCnum++;
            //if(setVCnum >= vcList.Length)
            //{
            //    // ����Ʈ ��ü ���̺��� ũ�� ����Ʈ�� Ŭ���� ù ��° �������� �����Ѵ�.
            //    videoPlayer.clip = vcList[0];
            //}
            //else
            //{
            //    // ����Ʈ ���̺��� ������ �ش� ��ȣ�� ������ �����Ѵ�.
            //    videoPlayer.clip = vcList[setVCnum];
            //}
        }
        else
        {
            // �迭�� ���� ������ ����Ѵ�
            setVCnum =  ((setVCnum - 1) + vcList.Length) % vcList.Length;
        }
        videoPlayer.clip = vcList[setVCnum];    // Ŭ���� �����Ѵ�.
        videoPlayer.Play();                     // �ٲ� Ŭ���� ����Ѵ�.
        currentVcIdx = setVCnum;                // �ٲ� Ŭ���� ������ ��ȣ�� ������Ʈ�Ѵ�.
    }

    public void SetVideoPlay(int num)
    {
        // ���� ������ι�ȣ�� ���޹��� ��ȣ�� �ٸ� ���� ����
        if(num != currentVcIdx)
        {
            videoPlayer.Stop();
            videoPlayer.clip = vcList[num];
            currentVcIdx = num;
            videoPlayer.Play();
        }
    }
}
