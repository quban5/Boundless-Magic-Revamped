using UnityEngine;

public class GroundScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private float width;

    void Start()
    {
        width = transform.localScale.x;
    }

    void Update()
    {
        if (playerTransform.position.x > transform.position.x)
        {
            transform.position += new Vector3(width/3,0);
        }
    }
}
