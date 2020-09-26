using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Koenig : Schachfigur {
  public override string Title {get{return "König";}}

  public override bool[,] PossibleMovements() {
    bool[,] moves = new bool[8,8];

    KingMove(X + 1, Z, ref moves); // hoch
    KingMove(X - 1, Z, ref moves); // runter
    KingMove(X, Z - 1, ref moves); // links
    KingMove(X, Z + 1, ref moves); // rechts
    KingMove(X + 1, Z - 1, ref moves); // links hoch
    KingMove(X - 1, Z - 1, ref moves); // links runter
    KingMove(X + 1, Z + 1, ref moves); // rechts hoch
    KingMove(X - 1, Z + 1, ref moves); // rechts runter

    return moves;
  }

  public void KingMove(int x, int z, ref bool[,] moves) {
    Schachfigur other;
    // wenn innerhalb des Spielfelds
    if (x >= 0 && x < 8 && z >= 0 && z < 8) {
      other = GameState.Instance.Schachfiguren[x,z];
      if (other == null) moves[x,z] = true;
      else if (this.isWhite != other.isWhite) moves[x,z] = true;
    }
  }
}
