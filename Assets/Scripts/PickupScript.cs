using System;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] private float speed;
    private CanvasManager canvasManager;
    [SerializeField] private CanvasManager.CatcherPositions position;
    public event Action addAmmoEvent;

    void Start()
    {
        canvasManager = GameObject.Find("Canvas").GetComponent<CanvasManager>();
        addAmmoEvent += () => GameManager.Instance.AddAmmo(2);
    }

    void Update()
    {
        transform.localPosition -= new Vector3(speed, 0, 0) * Time.unscaledDeltaTime;

        CanvasManager.CatcherPositions catcherPos = canvasManager.GetCatcherPos();

        if ((int)catcherPos == (int)position)
        {
            if (transform.position.x < 150)
            {
                addAmmoEvent?.Invoke();
                Destroy(gameObject);
            }
        }

        if (transform.position.x < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetPosition(CanvasManager.CatcherPositions position)
    {
        this.position = position;
    }
}
