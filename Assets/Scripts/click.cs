//该脚本对玩家对按钮的点击进行响应
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class click : MonoBehaviour
{
    // Start is called before the first frame update
    public void Click()//玩家点击退出按钮
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//编辑状态下退出
#else
        Application.Quit();//打包编译后退出
#endif

    }

    public void click2()//玩家点击显示最优路径（生命值）按钮
    {
        Text solu1 = GameManager.instance.solu1;
        if (solu1.text != "")
        {
            solu1.text = "";
            return;
        }
        solu1.text = "最优路径（生命值）：";
        List<string> paths = new List<string>();
        int end = GameManager.instance.castle_num - 1;
        while (end != -1)//路径还原
        {
            paths.Add(GameManager.instance.castle_names[end]);
            end = GameManager.instance.father[end];
        }
        paths.Reverse();
        for (int i = 0; i < paths.ToArray().Length; i++)
        {
            if (i == paths.ToArray().Length - 1)
            {
                solu1.text += paths[i];
                GameManager.instance.solution = solu1.text;
                return;
            }
            solu1.text += paths[i] + "->";//生成最优路径文本框
        }
    }
    public void click3()//玩家点击显示最优路径（财富值）按钮
    {
        Text solu1 = GameManager.instance.solu1;
        if (solu1.text != "")
        {
            solu1.text = "";
            return;
        }
        solu1.text = "最优路径（财富值）：";
        int end = GameManager.instance.castle_num - 1;
        int begin = 0;
        while (begin != end)//路径还原
        {
            solu1.text += GameManager.instance.castle_names[begin] + "->";
            begin = GameManager.instance.fortunepath[begin];
        }
        solu1.text += GameManager.instance.castle_names[begin];//生成最优路径文本框
        GameManager.instance.solution = solu1.text;
    }
}
