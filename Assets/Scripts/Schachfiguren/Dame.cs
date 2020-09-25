using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Dame : Schachfigur {
  public override string Title {get{return "Dame";}}

  public override bool[,] PossibleMovements() {
    bool[,] r = new bool[8,8];
        r[3,3] = true;
        return r;
  }
}
