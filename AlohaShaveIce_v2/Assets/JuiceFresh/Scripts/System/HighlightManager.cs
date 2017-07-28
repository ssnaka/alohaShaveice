using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighlightManager {
    static List<Item> crossedExtraItems = new List<Item>();
    static List<Item> extraItems = new List<Item>();
    static bool enableHighlights;

    public static void SelectItem(Item item) {
        ClearHighlight();
        if (item.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item.currentType == ItemsTypes.VERTICAL_STRIPPED)
            extraItems.Add(item);
        LightItem(item);
    }

    public static void DeselectItem(Item deleteItem, Item item) {
        ClearHighlight();
        if (deleteItem.currentType == ItemsTypes.HORIZONTAL_STRIPPED || deleteItem.currentType == ItemsTypes.VERTICAL_STRIPPED)
            extraItems.Remove(deleteItem);
        LightItem(item);
    }

    static void LightItem(Item item) {
        if (extraItems.Count == 1) {
            if (extraItems[0].currentType == ItemsTypes.HORIZONTAL_STRIPPED)
                LightRow(item.square);
            else if (extraItems[0].currentType == ItemsTypes.VERTICAL_STRIPPED)
                LightColumn(item.square);
        }
        else if (extraItems.Count >= 2) {
            LightCross(item.square);
        }
    }

    static void LightExtraItem(Item item) {
        if (item.currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            LightRow(item.square);
        else if (item.currentType == ItemsTypes.VERTICAL_STRIPPED)
            LightColumn(item.square);
    }

    static void LightCross(Square square) {
        LightRow(square);
        LightColumn(square);
    }

    static void LightRow(Square square) {
        List<Square> squareList = LevelManager.THIS.GetRowSquare(square.row);
        foreach (Square squareSelected in squareList) {
            squareSelected.HighLight(true);
            if (CheckCrossedItem(squareSelected))
                LightExtraItem(squareSelected.item);
        }
    }

    static void LightColumn(Square square) {
        List<Square> squareList = LevelManager.THIS.GetColumnSquare(square.col);
        foreach (Square squareSelected in squareList) {
            squareSelected.HighLight(true);
            if (CheckCrossedItem(squareSelected))
                LightExtraItem(squareSelected.item);
        }
    }

    static bool CheckCrossedItem(Square square) {
        if (square.IsExtraItem()) {
            if (crossedExtraItems.IndexOf(square.item) < 0 && extraItems.IndexOf(square.item) < 0) {
                crossedExtraItems.Add(square.item);
                return true;
            }
        }
        return false;
    }

    static void LightSquare(Square square) {
        square.HighLight(true);
    }

    static void ClearHighlight(bool boost = false) {

        //if (!boost)
        //    return;
        crossedExtraItems.Clear();
        List<Square> itemsList = LevelManager.THIS.GetSquares();
        foreach (Square square in itemsList) {
            square.HighLight(false);
        }
    }

    public static void StopAndClearAll() {
        extraItems.Clear();
        ClearHighlight();
    }

}
