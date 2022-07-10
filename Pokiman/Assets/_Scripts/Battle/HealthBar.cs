using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _healthBar;

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
}
