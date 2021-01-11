using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class click : MonoBehaviour
{
    // Start is called before the first frame update
    public void Click()
    {
        GameManager.instance.level = 0;
        GameManager.instance.playerFoodPoints = 100;
        GameManager.instance.playerTreasurePoints = 0;
        SceneManager.LoadScene(0);

    }
}
