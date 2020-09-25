using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Springer : Schachfigur {
  public override string Title {get{return "Springer";}}

  public override bool[,] PossibleMovements() {
    bool[,] r = new bool[8,8];
        r[3,3] = true;
        return r;
  }
}
