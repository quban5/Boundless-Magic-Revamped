using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float xOffset;

    void Update()
    {
        transform.position = new Vector3(playerTransform.position.x+xOffset,transform.position.y,-10);
    }
}
