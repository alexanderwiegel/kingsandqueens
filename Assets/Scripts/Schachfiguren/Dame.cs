using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Dame : Schachfigur {
  public override string Title {get{return "Dame";}}

  public override bool[,] PossibleMovements() {
    bool[,] moves = new bool[8,8];

    Schachfigur other;
    int i, j;

    #region rechts
    i = X;
    while (true) {
      i++;
      // außerhalb des Spielfelds
      if (i >= 8) break;

      other = GameState.Instance.Schachfiguren[i, Z];
      // wenn Feld frei ist, kann Königin dahin verschoben werden
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
      // wenn Feld frei ist, kann Königin dahin verschoben werden
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
      // wenn Feld frei ist, kann Königin dahin verschoben werden
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
      // wenn Feld frei ist, kann Königin dahin verschoben werden
      if (other == null) moves[X,i] = true;
      else {
        // wenn dort ein Gegner steht
        if (this.isWhite != other.isWhite) moves[X,i] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region diagonal hoch links
    i = X;
    j = Z;

    while (true) {
      i--;
      j++;
      if (i < 0 || j >= 8) break;
      other = GameState.Instance.Schachfiguren[i,j];
      // wenn Feld frei ist, kann Königin dahin verschoben werden
      if (other == null) moves[i,j] = true;
      else {
        // wenn dort ein Gegner steht ebenfalls
        if (this.isWhite != other.isWhite) moves[i,j] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region diagonal hoch rechts
    i = X;
    j = Z;

    while (true) {
      i++;
      j++;
      if (i >= 8 || j >= 8) break;
      other = GameState.Instance.Schachfiguren[i,j];
      // wenn Feld frei ist, kann Königin dahin verschoben werden
      if (other == null) moves[i,j] = true;
      else {
        // wenn dort ein Gegner steht ebenfalls
        if (this.isWhite != other.isWhite) moves[i,j] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region diagonal runter links
    i = X;
    j = Z;

    while (true) {
      i--;
      j--;
      if (i < 0 || j < 0) break;
      other = GameState.Instance.Schachfiguren[i,j];
      // wenn Feld frei ist, kann Königin dahin verschoben werden
      if (other == null) moves[i,j] = true;
      else {
        // wenn dort ein Gegner steht ebenfalls
        if (this.isWhite != other.isWhite) moves[i,j] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion

    #region diagonal runter rechts
    i = X;
    j = Z;

    while (true) {
      i++;
      j--;
      if (i >= 8 || j < 0) break;
      other = GameState.Instance.Schachfiguren[i,j];
      // wenn Feld frei ist, kann Königin dahin verschoben werden
      if (other == null) moves[i,j] = true;
      else {
        // wenn dort ein Gegner steht ebenfalls
        if (this.isWhite != other.isWhite) moves[i,j] = true;
        // ob Freund oder Feind, weiter geht es nicht
        break;
      }
    }
    #endregion
    
    return moves;
  }
}