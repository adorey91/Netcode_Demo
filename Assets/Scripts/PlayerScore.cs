using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameUI;
    [SerializeField] private TextMeshProUGUI scoreUI;

    public void TrackPlayer(GameObject player)
    {
        player.GetComponent<PlayerData>().Name.OnValueChanged += OnNameChanged;
        player.GetComponent<PlayerData>().Score.OnValueChanged += OnScoreChanged;
        OnScoreChanged(0, player.GetComponent<PlayerData>().Score.Value);
        OnNameChanged("",  player.GetComponent<PlayerData>().Name.Value);
    }

    private void OnScoreChanged(int previousvalue, int newvalue)
    {
        scoreUI.text = $"Score: {newvalue}";
    }

    private void OnNameChanged(FixedString128Bytes previousvalue, FixedString128Bytes newvalue)
    {
        nameUI.text = $"Name: {newvalue}";
    }
}
