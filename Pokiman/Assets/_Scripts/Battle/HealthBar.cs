using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _healthBar;

    public Color BarColor
    {
        get
        {
            var localScale = _healthBar.transform.localScale.x;
            if (localScale < 0.15f)
            {
                return new Color(1.0f, 0, 0, 1.0f);
            }
            else if (localScale < 0.5f)
            {
                return new Color(1.0f, 1.0f, 0, 1.0f);
            }
            else
            {
                return new Color(0.45f, 0.68f, 0.19f, 1.0f);
            }
        }
    }

    /*private void Start()
    {
        _healthBar.transform.localScale = new Vector3(0.5f, 1.0f, 1.0f);
    }*/

    /// <summary>actualiza la barra de vida a partir del valor normalizado de la misma</summary>
    /// <param name="pNormalizedValue">valor de la vida normalizado entre 0 y 1</param>
    public void SetHealthBar(float pNormalizedValue)
    {
        _healthBar.transform.localScale = new Vector3(pNormalizedValue, 1.0f, 1.0f);
    }

    public IEnumerator SetSmoothHP(float pNormalizedValue)
    {
        float currentScale = _healthBar.transform.localScale.x;
        float updateQuantity = currentScale - pNormalizedValue;
        while (currentScale - pNormalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            _healthBar.transform.localScale = new Vector3(currentScale, 1.0f, 1.0f);
            _healthBar.GetComponent<Image>().color = BarColor;
            yield return null;
        }
        _healthBar.transform.localScale = new Vector3(pNormalizedValue, 1.0f, 1.0f);
    }
}
