using UnityEngine;
using TMPro;

public class UiPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerText;

    private void ChangeText(int health)
    {
        playerText.text = $"Player Health: {health}";
    }
}
