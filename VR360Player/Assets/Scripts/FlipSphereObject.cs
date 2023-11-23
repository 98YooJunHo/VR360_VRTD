using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSphereObject : MonoBehaviour
{
    // Flip �� sphereObj ĳ��
    private GameObject flipObj = default;

    private void Awake()
    {
        flipObj = gameObject;
        FlipObject();
    }

    //! ������Ʈ�� mesh���� �������� �����´�. �������� vertex�� ����� mesh�� flip�ϴ� �Լ�
    private void FlipObject()
    {
        // { �޽� ������ ������ ���� ���ϴ� ����
        MeshFilter meshFilter = flipObj.GetComponent<MeshFilter>();
        Vector3[] normals = meshFilter.mesh.normals;
        Debug.LogFormat("�޽� �������� ����: {0}", normals.Length);

        for(int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }

        meshFilter.mesh.normals = normals;
        // } �޽� ������ ������ ���� ���ϴ� ����

        // { �������� �����ϴ� �ﰢ���� �� ���߿� ����� ������ ������ �� ���� Swap �Ͽ� ������ ����
        int[] triangles = meshFilter.mesh.triangles;
        int tempTriangle = default;
        Debug.LogFormat("�ﰢ���� ����: {0}", triangles.Length);

        for (int i = 0; i < triangles.Length; i += 3)
        {
            tempTriangle = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = tempTriangle;
        }

        meshFilter.mesh.triangles = triangles;
        // } �������� �����ϴ� �ﰢ���� �� ���߿� ����� ������ ������ �� ���� Swap �Ͽ� ������ ����
    }
}
