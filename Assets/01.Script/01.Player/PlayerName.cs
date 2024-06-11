// # System
using System.Collections;
using System.Collections.Generic;
using TMPro;

// # Unity
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI playerName;

    private void Awake()
    {
        playerName = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        playerName.text = GameManager.instance.playerID;
    }
}