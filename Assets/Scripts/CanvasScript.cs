using UnityEngine;
using TMPro;

public class CanvasScript : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    void Update()
    {
        scoreText.text ="Score: " + GameManager.Instance.GetScore().ToString();
    }
}
