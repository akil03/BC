using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public static DebugText instance;
    public Text text;

    private void Awake()
    {
        instance = this;
    }


    public void Print(string str)
    {
        text.text = str;
        print(str);
    }
}
