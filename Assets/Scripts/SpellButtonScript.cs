using UnityEngine;

public class SpellButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerCastSpell()
    {
        if (GameManager.Instance != null)
        {
            SpellManager spellManager = GameManager.Instance.GetComponent<SpellManager>();
            spellManager.CastSpell();
        }
    }
}
