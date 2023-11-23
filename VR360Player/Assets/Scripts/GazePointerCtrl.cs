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
        // ĵ���� ������Ʈ�� �������� �Ÿ��� ���� �����Ѵ�
        // 1. ī�޶� �������� ���� ������ ��ǥ�� ���Ѵ�
        Vector3 dir = transform.TransformPoint(Vector3.forward);
        // 2. ī�޶� �������� ������ ���̸� �����Ѵ�.
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo; // ��Ʈ�� ������Ʈ�� ������ ĳ���Ѵ�
        // 3. ���̿� �ε��� ��쿡�� �Ÿ� ���� �̿��� uiCanvas�� ũ�⸦ �����Ѵ�.
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
        else // 4. �ƹ��͵� �ε����� ������ �⺻ ������ ������ uiCanvas�� ũ�⸦ �����Ѵ�.
        {
            uiCanvas.localScale = defaultScale * uiScaleVal;
            uiCanvas.position = transform.position + dir;
        }
        // 5. uiCanvas�� �׻� ī�޶� ������Ʈ�� �ٶ󺸰� �Ѵ�
        uiCanvas.forward = transform.forward * -1;

        // GazeObj�� ���̰� ����� �� ����
        if(isHitObj)
        {
            if(currentHitObj == prevHitObj)     // ���� �����Ӱ� ���� �������� ���� ������Ʈ�� ���ƾ� �ð� ����
            {
                // ���ͷ����� �߻��ؾ� �ϴ� ������Ʈ�� �ü��� ������ �ִٸ� �ð� ����
                currentGazeTime += Time.deltaTime;
            }
            else                                // ���� �������� ������Ʈ�� ���� �������� ������Ʈ���� ��� ���
            {
                // ���� �������� ���� ������ ������Ʈ�Ѵ�.
                prevHitObj = currentHitObj;
            }
            // hit�� ������Ʈ�� VideoPlayer ������Ʈ�� ���� �ִ��� Ȯ���Ѵ�
            HitObjChecker(currentHitObj, true);
        }
        else        // �ü��� ����ų� GazeObj�� �ƴ� ������Ʈ�� �ٶ󺸴� ���
        {
            if(prevHitObj != null)
            {
                HitObjChecker(prevHitObj, false);
                prevHitObj = null;  // prevHitObj�� ������ �����
            }
            currentGazeTime = 0;
        }
        // �ü��� �ӹ� �ð��� 0�� �ִ� ���̷� �Ѵ�
        currentGazeTime = Mathf.Clamp(currentGazeTime, 0, gazeChargeTime);
        // ui Image�� fillAmount�� ������Ʈ�Ѵ�.
        gazeImg.fillAmount = currentGazeTime / gazeChargeTime;

        // gazePointer�� �������� �� ������ ��ŭ �ø� ������ ���� �����ӿ� ���� �������� �ʱ�ȭ�Ѵ�.
        isHitObj = false;       // ��� ó���� ������ isHitObj�� false�� �Ѵ�.
        currentHitObj = null;   // currentHitObj�� ������ �����.
    }

    // ��Ʈ�� ������Ʈ Ÿ�Ժ��� �۵� ����� �����Ѵ�.
    void HitObjChecker(GameObject hitObj, bool isActive)
    {
        // hit�� ���� �÷��̾� ������Ʈ�� ���� �ִ��� Ȯ���Ѵ�.
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
