using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static bool GameActive = false;

    public static void GameStart()
    {
        GameActive = true;
    }
}
