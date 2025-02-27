using UnityEngine;
using TMPro;

public class UiPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerText;

    #region Events
    private void OnEnable()
    {
        HealthSystem.OnHealthChanged += ChangeText;
    }

    private void OnDisable()
    {
        HealthSystem.OnHealthChanged -= ChangeText;
    }

    private void OnDestroy()
    {
        HealthSystem.OnHealthChanged -= ChangeText;
    }
    #endregion

    private void ChangeText(ushort health)
    {
        playerText.text = $"Player Health: {health.ToString()}";
    }
}
