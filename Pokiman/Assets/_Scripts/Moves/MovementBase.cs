using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Scriptable object con el que definimos los movimientos de los pokemon y sus estadisticas y tipo
[CreateAssetMenu(fileName ="Move", menuName = "Pokemon/Nuevo Movimiento")]
public class MovementBase : ScriptableObject
{
    [SerializeField] private string namae;
    public string Namae => namae;
    [SerializeField][TextArea] private string description;
    public string Description => description;
    [SerializeField] private PokemonType tipoAtaque;
    public PokemonType TipoAtaque => tipoAtaque;
    [SerializeField] private int power;
    public int Power => power;
    [SerializeField] private int accuracy;
    public int Accuracy => accuracy;
    [SerializeField] private int pp;
    public int Pp =>  pp;
}
