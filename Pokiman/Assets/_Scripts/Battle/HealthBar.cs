using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _healthBar;

    public Color BarColor(float finalScale)
    {
        if (finalScale < 0.15f)
        {
            return new Color(1.0f, 0, 0, 1.0f);
        }
        else if (finalScale < 0.5f)
        {
            return new Color(1.0f, 1.0f, 0, 1.0f);
        }
        else
        {
            return new Color(0.45f, 0.68f, 0.19f, 1.0f);
        }
    }

    /// <summary>actualiza la barra de vida a partir del valor normalizado de la misma</summary>
    /// <param name="pNormalizedValue">valor de la vida normalizado entre 0 y 1 </param>
    public void SetHealthBar(float pNormalizedValue)
    {
        _healthBar.transform.localScale = new Vector3(pNormalizedValue, 1.0f, 1.0f);
        _healthBar.GetComponent<Image>().color = BarColor(pNormalizedValue);
    }

    public IEnumerator SetSmoothHP(float pNormalizedValue)
    {
        var seq = DOTween.Sequence();
        seq.Append(_healthBar.transform.DOScaleX(pNormalizedValue, 1.5f));
        seq.Join(_healthBar.GetComponent<Image>().DOColor(BarColor(pNormalizedValue), 1f));
        yield return seq.WaitForCompletion();

        // float currentScale = _healthBar.transform.localScale.x;
        // float updateQuantity = currentScale - pNormalizedValue;
        // while (currentScale - pNormalizedValue > Mathf.Epsilon)
        // {
        //     currentScale -= updateQuantity * Time.deltaTime;
        //     _healthBar.transform.localScale = new Vector3(currentScale, 1.0f, 1.0f);
        //     barColor.color = BarColor;
        //     yield return null;
        // }
        // _healthBar.transform.localScale = new Vector3(pNormalizedValue, 1.0f, 1.0f);
    }
}
