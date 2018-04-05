using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;


public class DeadUI : MonoBehaviour
{
    public Text usernameText;
    public Text rankText;
    public Text killText;
    public Text rewardText;

    public void Setup(JSONObject jsonData)
    {
        usernameText.text = jsonData[0].ToString();
        rankText.text = string.Format("Rank #{0}", jsonData[1]);
        killText.text = string.Format("Kill {0}", jsonData[2]);
        rewardText.text = string.Format("Reward {0}", jsonData[3]);
    }
    
}
