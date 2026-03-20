using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayUIManager : MonoBehaviour
{
    public static GamePlayUIManager Instance;

    [SerializeField]
    TextMeshProUGUI remainCard;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetRemainCard(int remain)
    {
        remainCard.text = "สฃำเลฦสýฃบ" + remain.ToString();
    }
}
