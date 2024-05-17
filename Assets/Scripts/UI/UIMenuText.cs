using TMPro;
using UnityEngine;

public class UIMenuText : MonoBehaviour
{
    [SerializeField] private TMP_Text HealthText;
    [SerializeField] private Player health;

    private void OnEnable()
    {
        health.onHPChange += HandleUpdateHealthText;
    }

    private void OnDisable()
    {
        health.onHPChange -= HandleUpdateHealthText;
    }

    private void HandleUpdateHealthText()
    {
        HealthText.text = "health: " + health.HealthPoints;
    }
}
