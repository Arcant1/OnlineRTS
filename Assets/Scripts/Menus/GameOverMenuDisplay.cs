using TMPro;
using UnityEngine;
using Mirror;

public class GameOverMenuDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayParent;
    [SerializeField] private TMP_Text winnerNameText;
    private void OnEnable()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }
    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            // stop hosting
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // stop client
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string winnerName)
    {
        winnerNameText.text = $"Player {winnerName} ";
        gameOverDisplayParent.SetActive(true);
    }
}
