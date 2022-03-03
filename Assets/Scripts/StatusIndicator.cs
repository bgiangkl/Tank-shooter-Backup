using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{

    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("No healthBar reference");
        }
        if (healthText == null)
        {
            Debug.LogError("No healthText reference");
        }
    }

    public void setHealth(int _cur, int _max)
    {
        // Underscore indicates that the scope of the variable is only within this function
        float _value = (float)_cur / _max;

        healthBar.localScale = new Vector3(_value, healthBar.localScale.y, healthBar.localScale.z);
        healthText.text = _cur + "/" + _max + " HP";

    }
}
