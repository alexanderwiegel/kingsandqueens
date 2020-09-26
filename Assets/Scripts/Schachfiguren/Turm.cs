class Turm : Schachfigur {
  public override string Title {get{return "Turm";}}

  public override bool[,] PossibleMovements() {
    Schachfigur other;

    bool[,] moves = new bool[8,8];
    int i;

    #region rechts
    i = X;
    while (true) {
      i++;
      // außerhalb des Spielfelds
      if (i >= 8) break;

      other = GameState.Instance.Schachfiguren[i, Z];
      // wenn Feld frei ist, kann Turm dahin verschoben werden
      if (other == null) moves[i,Z] = true;
      else {
        // wenn dort ein Gegner steht ebenfalls
        if (this.isWhite != other.isWhite) moves[i,Z] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region links
    i = X;
    while (true) {
      i--;
      // außerhalb des Spielfelds
      if (i < 0) break;

      other = GameState.Instance.Schachfiguren[i, Z];
      // wenn Feld frei ist, kann Turm dahin verschoben werden
      if (other == null) moves[i,Z] = true;
      else {
        // wenn dort ein Gegner steht
        if (this.isWhite != other.isWhite) moves[i,Z] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region hoch
    i = Z;
    while (true) {
      i++;
      // außerhalb des Spielfelds
      if (i >= 8) break;

      other = GameState.Instance.Schachfiguren[X, i];
      // wenn Feld frei ist, kann Turm dahin verschoben werden
      if (other == null) moves[X,i] = true;
      else {
        // wenn dort ein Gegner steht
        if (this.isWhite != other.isWhite) moves[X,i] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region runter
    i = Z;
    while (true) {
      i--;
      // außerhalb des Spielfelds
      if (i < 0) break;

      other = GameState.Instance.Schachfiguren[X, i];
      // wenn Feld frei ist, kann Turm dahin verschoben werden
      if (other == null) moves[X,i] = true;
      else {
        // wenn dort ein Gegner steht
        if (this.isWhite != other.isWhite) moves[X,i] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    return moves;
  }
}
