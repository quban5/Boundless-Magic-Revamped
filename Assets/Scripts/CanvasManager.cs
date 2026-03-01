using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    void Update()
    {
        //Set the score text
        scoreText.text = "Score: " + GameManager.Instance.GetScore().ToString();
    }
}
