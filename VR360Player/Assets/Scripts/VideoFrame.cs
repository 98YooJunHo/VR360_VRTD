using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoFrame : MonoBehaviour
{
    VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        // ���� ������Ʈ�� ���� �÷��̾� ������Ʈ ������ ������ �´�.
        videoPlayer = GetComponent<VideoPlayer>();
        // �ڵ� ����Ǵ� ���� ���´�.
        videoPlayer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // s�� ������ �����϶�.
        if(Input.GetKeyDown(KeyCode.S))
        {
            videoPlayer.Stop();
        }

        // �����̽� �ٸ� ������ �� ��� �Ǵ� �Ͻ������� �϶�.
        if(Input.GetKeyDown("space"))
        {
            // ���� ���� �÷��̾ �÷��� �������� Ȯ���϶�.
            if(videoPlayer.isPlaying)
            {
                // �÷���(���) ���̶�� �Ͻ����� �϶�.
                videoPlayer.Pause();
            }
            else
            {
                // �׷��� �ʴٸ�(�Ͻ����� Ȥ�� ����) �÷���(���)�϶�.
                videoPlayer.Play();
            }
        }
    }
    
    // GazePointerCtrl���� ���� ����� ��Ʈ���ϱ� ���� �Լ�
    public void CheckVideoFrame(bool checker)
    {
        if (checker)
        {
            if(!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
        else
        {
            videoPlayer.Stop();
        }
    }
}
