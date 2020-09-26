using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Koenig : Schachfigur {
  public override string Title {get{return "König";}}

  public override bool[,] PossibleMovements() {
    bool[,] moves = new bool[8,8];

    Schachfigur other;
    int i, j;

    #region hoch
    i = X-1;
    j = Z+1;
    // wenn nicht am oberen Spielfeldrand
    if (Z != 7) {
      // 1. hoch-links, 2. hoch-mitte, 3. hoch-rechts
      for (int k = 0; k < 3; k++) {
        if (i >= 0 && i < 8) {
          other = GameState.Instance.Schachfiguren[i,j];
          // wenn Feld frei ist, kann König dahin verschoben werden
          if (other == null) moves[i,j] = true;
          // wenn dort ein Gegner steht ebenfalls
          else if (this.isWhite != other.isWhite) moves[i,j] = true;
        }
        i++;
      }
    }
    #endregion

    #region runter
    i = X-1;
    j = Z-1;
    // wenn nicht am unteren Spielfeldrand
    if (Z != 0) {
      // 1. runter-links, 2. runter-mitte, 3. runter-rechts
      for (int k = 0; k < 3; k++) {
        if (i >= 0 && i < 8) {
          other = GameState.Instance.Schachfiguren[i,j];
          // wenn Feld frei ist, kann König dahin verschoben werden
          if (other == null) moves[i,j] = true;
          // wenn dort ein Gegner steht ebenfalls
          else if (this.isWhite != other.isWhite) moves[i,j] = true;
        }
        i++;
      }
    }
    #endregion

    #region links
    // wenn nicht am linken Spielfeldrand
    if (X != 0) {
      other = GameState.Instance.Schachfiguren[X-1, Z];
      // wenn Feld frei ist, kann König dahin verschoben werden
      if (other == null) moves[X-1, Z] = true;
      // wenn dort ein Gegner steht ebenfalls
      else if (this.isWhite != other.isWhite) moves[X-1, Z] = true;
    }
    #endregion

    #region rechts
    // wenn nicht am rechten Spielfeldrand
    if (X != 7) {
      other = GameState.Instance.Schachfiguren[X+1, Z];
      // wenn Feld frei ist, kann König dahin verschoben werden
      if (other == null) moves[X+1, Z] = true;
      // wenn dort ein Gegner steht ebenfalls
      else if (this.isWhite != other.isWhite) moves[X+1, Z] = true;
    }
    #endregion

    return moves;
  }
}
