using System.Numerics;
using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }

        float speed = GameManager.Instance.GetCurrentSpeed();
        transform.Translate(UnityEngine.Vector3.left * speed * Time.deltaTime);
    }
}
