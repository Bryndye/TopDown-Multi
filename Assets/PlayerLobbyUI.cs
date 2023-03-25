using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
public class PlayerLobbyUI : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI ReadyText;

    public void SetInfoOnUIPLyaer(Player player)
    {
        PlayerName.text = $"{player.Data["PlayerName"].Value}";
        ReadyText.text = "NO VAR";
    }
}
