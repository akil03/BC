using UnityEngine;
using UnityEngine.UI;

public class UINamer : MonoBehaviour
{
    public string Name;
    Text text;

    private void OnValidate()
    {
        if (text == null)
        {
            text = transform.GetChild(0).GetComponent<Text>();
        }
        if (gameObject.name != Name)
        {
            gameObject.name = Name;
            text.text = Name;
        }
    }
}