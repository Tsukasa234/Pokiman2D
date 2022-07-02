using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    //referencia al scriptable object de los movimientos
    private MovementBase _base;
    //Variable para medir los puntos de poder
    private int _pp;

    public MovementBase Base {get => _base; set => _base = value;}
    public int PP {get => _pp; set => _pp = value;}

    //Constructor de la clase en el que damos valores a las 2 variables anteriores
    public Move(MovementBase pBase)
    {
        _base = pBase;
        _pp = _base.Pp;
    }
}
