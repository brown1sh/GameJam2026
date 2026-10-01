using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public void OnEndTurnButtonPressed()
    {
        TurnManager.Instance.EndTurn();
    }
}
