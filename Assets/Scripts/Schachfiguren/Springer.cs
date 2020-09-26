class Springer : Schachfigur {
  public override string Title {get{return "Springer";}}

  public override bool[,] PossibleMovements() {
    bool[,] moves = new bool[8,8];

    // hoch-links
    Sprung(X-1, Z+2, ref moves);

    // hoch-rechts
    Sprung(X+1, Z+2, ref moves);

    // links-hoch
    Sprung(X-2, Z+1, ref moves);

    // rechts-hoch
    Sprung(X+2, Z+1, ref moves);

    // runter-links
    Sprung(X-1, Z-2, ref moves);

    // runter-rechts
    Sprung(X+1, Z-2, ref moves);

    // links-runter
    Sprung(X-2, Z-1, ref moves);

    // rechts-runter
    Sprung(X+2, Z-1, ref moves);

    return moves;
  }

  // ref-Stichwort, damit moves by reference und nicht by value übergeben wird
  public void Sprung(int x, int z, ref bool[,] moves) {
    Schachfigur other;

    // wenn Feld innerhalb des Spielfelds
    if (x >= 0 && x < 8 && z >= 0 && z < 8) {
      other = GameState.Instance.Schachfiguren[x,z];
      // wenn Feld frei ist, kann Springer dahin verschoben werden
      if (other == null) moves[x,z] = true;
      // wenn dort ein Gegner steht ebenfalls
      else if (this.isWhite && !other.isWhite) moves[x,z] = true;
    }
  }
}
