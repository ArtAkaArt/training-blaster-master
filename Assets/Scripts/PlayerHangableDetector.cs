using UnityEngine;


//����������� ������� � ��������, ������� ��� �������
public class PlayerHangableDetector : MonoBehaviour
{
    private bool isHangableNearby = false;
    private GameObject hangableObject = null;

    public bool IsHangableNearby()
    {
        return isHangableNearby;
    }

    public GameObject GetHangableObject()
    {
        return hangableObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hangable"))
        {
            isHangableNearby = true;
            hangableObject = collision.gameObject;
            Debug.Log("����� ���������� ��������", hangableObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Hangable"))
        {
            isHangableNearby = false;
            hangableObject = null;
        }
    }
}