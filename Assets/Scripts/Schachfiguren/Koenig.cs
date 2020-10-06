class Koenig : Schachfigur {
  public override string Title {get{return "König";}}
  GameState gs = GameState.Instance;

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

    #region Rochade
    // wenn sich der König noch nicht bewegt hat und nicht bedroht ist
    if (!hasMoved && isWhite && !gs.blackMoves[X,Z] || !isWhite && !gs.whiteMoves[X,Z]) {

      Schachfigur other;

      #region kurze Rochade
      other = gs.Schachfiguren[X+3,Z];
      // wenn der Königsturm sich noch nicht bewegt hat
      if (other.Title == "Turm" && !other.hasMoved) {
        // und alle Felder dazwischen frei
        if (gs.Schachfiguren[X+1,Z] == null
         && gs.Schachfiguren[X+2,Z] == null) {
          // und nicht vom Gegner bedroht sind
          bool[,] enemyMoves;
          if (this.isWhite) enemyMoves = gs.blackMoves;
          else enemyMoves = gs.whiteMoves;
          if (!enemyMoves[X+1,Z] && !enemyMoves[X+2,Z]) {
          // dann ist die kurze Rochade möglich
          moves[X+2,Z] = true;
          }
        }
      }
      #endregion

      #region lange Rochade
      other = gs.Schachfiguren[X-4,Z];
      // wenn der Damenturm sich noch nicht bewegt hat
      if (other.Title == "Turm" && !other.hasMoved) {
        // und alle Felder dazwischen frei
        if (gs.Schachfiguren[X-1,Z] == null
         && gs.Schachfiguren[X-2,Z] == null
         && gs.Schachfiguren[X-3,Z] == null) {
          // und nicht vom Gegner bedroht sind
          bool[,] enemyMoves;
          if (this.isWhite) enemyMoves = gs.blackMoves;
          else enemyMoves = gs.whiteMoves;
          if (!enemyMoves[X-1,Z] && !enemyMoves[X-2,Z] && !enemyMoves[X-3,Z]) {
            // dann ist die lange Rochade möglich
            moves[X-2,Z] = true;
          }
        }
      }
      #endregion
    }
    #endregion

    return moves;
  }

  public void KingMove(int x, int z, ref bool[,] moves) {
    Schachfigur other;
    // wenn innerhalb des Spielfelds
    if (x >= 0 && x < 8 && z >= 0 && z < 8) {
      other = gs.Schachfiguren[x,z];
      // und das Feld frei
      if (other == null) {
        // und das Feld nicht vom Gegner bedroht ist
        if (this.isWhite && !gs.blackMoves[x,z] || !this.isWhite && !gs.whiteMoves[x,z])
        moves[x,z] = true;
      }
      // oder auf dem Feld ein Gegner steht
      else if (this.isWhite != other.isWhite) moves[x,z] = true;
    }
  }
}
