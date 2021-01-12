using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class click : MonoBehaviour
{

    // Start is called before the first frame update
    public void Click()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//编辑状态下退出
#else
        Application.Quit();//打包编译后退出
#endif

    }

    public void click2()
    {
        Text solu1 = GameManager.instance.solu1;
        if(solu1.text!="")
        {
            solu1.text = "";
            return;
        }
        List<string> paths = new List<string>();
        int end = GameManager.instance.castle_num - 1;
        while (end != -1)
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
                return;
            }
            solu1.text += paths[i] + "->";
        }
        GameManager.instance.solution = solu1.text;
    }

    public void click3()
    {
        Text solu1 = GameManager.instance.solu1;
        if(solu1.text!="")
        {
            solu1.text = "";
            return;
        }
        int end = GameManager.instance.castle_num - 1;
        int begin = 0;
        while(begin!=end)
        {
            solu1.text+=GameManager.instance.castle_names[begin] + "->";
            begin = GameManager.instance.fortunepath[begin];
        }
        solu1.text+=GameManager.instance.castle_names[begin];
        GameManager.instance.solution = solu1.text;
    }

}
