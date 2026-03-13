using UnityEngine;
using TMPro;

public class FlickerText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 0.1f;

    void Update()
    {
        text.alpha = Random.Range(0.7f, 1f);
    }
}

