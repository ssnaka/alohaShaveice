using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum FindSeparating {
    NONE = 0,
    HORIZONTAL,
    VERTICAL
}

public enum SquareTypes {
    NONE = 0,
    EMPTY,
    BLOCK,
    WIREBLOCK,
    SOLIDBLOCK,
    DOUBLESOLIDBLOCK,
    DOUBLEBLOCK,
    UNDESTROYABLE,
    THRIVING
}



public class Square : MonoBehaviour {
    public Square square;
    public Item item;
    public int row;
    public int col;
    public SquareTypes type;

    public List<GameObject> block = new List<GameObject>();
    public int cageHP;
    public int cageHPPreview;
    private bool cageActive;
    private int OldcageHP;
    public GameObject boomPrefab;
    public GameObject icePrefab;

    // Use this for initialization
    void Start() {
        // GenItem();
        square = this;
        if (row == LevelManager.THIS.maxRows - 1) {
            // if (LevelManager.THIS.target == Target.COLLECT && (LevelManager.THIS.ingrTarget[0] == Ingredients.Ingredient1 || LevelManager.THIS.ingrTarget[1] == Ingredients.Ingredient1 || LevelManager.THIS.ingrTarget[0] == Ingredients.Ingredient2 || LevelManager.THIS.ingrTarget[1] == Ingredients.Ingredient2))
            // {
            //     //GameObject obj = Instantiate( Resources.Load("Prefabs/arrow_ingredients")) as GameObject;
            //     //obj.transform.SetParent(transform);
            //     //obj.transform.localPosition = Vector3.zero + Vector3.down * 0.8f;
            // }
        }

    }

    public Item GenItem(bool falling = true) {
        if (IsNone() && !CanGoInto())
            return null;
        GameObject item = Instantiate(LevelManager.THIS.itemPrefab) as GameObject;
        item.transform.localScale = Vector2.one * 0.9f;
        item.GetComponent<Item>().square = this;
        //if (!falling)
        //    item.GetComponent<Item>().anim.SetTrigger("reAppear");

        item.transform.SetParent(transform.parent);
        if (falling) {
            item.transform.position = transform.position + Vector3.back * 0.2f + Vector3.up * 3f;
            item.GetComponent<Item>().justCreatedItem = true;
        }
        else
            item.transform.position = transform.position + Vector3.back * 0.2f;
        this.item = item.GetComponent<Item>();
        return this.item;
    }

    public Square GetNeighborLeft(bool safe = false) {
        if (col == 0 && !safe)
            return null;
        return LevelManager.THIS.GetSquare(col - 1, row, safe);
    }

    public Square GetNeighborRight(bool safe = false) {
        if (col >= LevelManager.THIS.maxCols && !safe)
            return null;
        return LevelManager.THIS.GetSquare(col + 1, row, safe);
    }

    public Square GetNeighborTop(bool safe = false) {
        if (row == 0 && !safe)
            return null;
        return LevelManager.THIS.GetSquare(col, row - 1, safe);
    }

    public Square GetNeighborBottom(bool safe = false) {
        if (row >= LevelManager.THIS.maxRows && !safe)
            return null;
        return LevelManager.THIS.GetSquare(col, row + 1, safe);
    }

    Hashtable FindMoreMatches(int spr_COLOR, Hashtable countedSquares, FindSeparating separating, Hashtable countedSquaresGlobal = null) {
        bool globalCounter = true;
        if (countedSquaresGlobal == null) {
            globalCounter = false;
            countedSquaresGlobal = new Hashtable();
        }

        if (this.item == null)
            return countedSquares;
        if (this.item.destroying)
            return countedSquares;
        //    if (LevelManager.THIS.countedSquares.ContainsValue(this.item) && globalCounter) return countedSquares;
        if (this.item.color == spr_COLOR && !countedSquares.ContainsValue(this.item) && this.item.currentType != ItemsTypes.INGREDIENT) {
            if (LevelManager.THIS.onlyFalling && this.item.justCreatedItem)
                countedSquares.Add(countedSquares.Count - 1, this.item);
            else if (!LevelManager.THIS.onlyFalling)
                countedSquares.Add(countedSquares.Count - 1, this.item);
            else
                return countedSquares;

            if (separating == FindSeparating.HORIZONTAL) {
                if (GetNeighborLeft() != null)
                    countedSquares = GetNeighborLeft().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
                if (GetNeighborRight() != null)
                    countedSquares = GetNeighborRight().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.HORIZONTAL);
            }
            else if (separating == FindSeparating.VERTICAL) {
                if (GetNeighborTop() != null)
                    countedSquares = GetNeighborTop().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
                if (GetNeighborBottom() != null)
                    countedSquares = GetNeighborBottom().FindMoreMatches(spr_COLOR, countedSquares, FindSeparating.VERTICAL);
            }
        }
        return countedSquares;
    }

    public List<Item> FindMatchesAround(FindSeparating separating = FindSeparating.NONE, int matches = 3, Hashtable countedSquaresGlobal = null) {
        bool globalCounter = true;
        List<Item> newList = new List<Item>();
        if (countedSquaresGlobal == null) {
            globalCounter = false;
            countedSquaresGlobal = new Hashtable();
        }
        Hashtable countedSquares = new Hashtable();
        countedSquares.Clear();
        if (this.item == null)
            return newList;
        if (separating != FindSeparating.VERTICAL) {
            countedSquares = this.FindMoreMatches(this.item.color, countedSquares, FindSeparating.HORIZONTAL, countedSquaresGlobal);
        }
        foreach (DictionaryEntry de in countedSquares) {
            LevelManager.THIS.countedSquares.Add(LevelManager.THIS.countedSquares.Count - 1, de.Value);
        }

        if (countedSquares.Count < matches)
            countedSquares.Clear();

        if (separating != FindSeparating.HORIZONTAL) {
            countedSquares = this.FindMoreMatches(this.item.color, countedSquares, FindSeparating.VERTICAL, countedSquaresGlobal);
        }
        foreach (DictionaryEntry de in countedSquares) {
            LevelManager.THIS.countedSquares.Add(LevelManager.THIS.countedSquares.Count - 1, de.Value);
        }

        if (countedSquares.Count < matches)
            countedSquares.Clear();
        foreach (DictionaryEntry de in countedSquares) {
            newList.Add((Item)de.Value);
        }
        // print(countedSquares.Count);
        return newList;
    }

    // Update is called once per frame
    void Update() {
        canDamage = cageActive;
    }

    void LateUpdate() {
        cageHPPreview = cageHP;
        //if (cageActive && !justHighlighted)
        //{
        //    cageHPPreview = Mathf.Clamp(cageHP - LevelManager.THIS.destroyAnyway.Count, 0, cageHP);
        //    print(cageHPPreview);
        //}
        //else if (justHighlighted)
        //{
        //    cageHPPreview = Mathf.Clamp(cageHP - 5, 0, cageHP);
        //    print(cageHPPreview);
        //}
    }

    bool hightlighted;

    public void HighLight(bool on, float col = 0.5f) {
        if (on) {
            GetComponent<SpriteRenderer>().color = new Color(col, col, col);
            if (!hightlighted)
                StartCoroutine(AnimateHighlight(col));
        }
        else {
            StopCoroutine(AnimateHighlight(col));
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }
        hightlighted = on;
        SetActiveCage(on);
    }

    IEnumerator AnimateHighlight(float col = 0.5f) {
        GetComponent<SpriteRenderer>().color = new Color(col, col, col);
        while (hightlighted) {
            yield return new WaitForSeconds(0.3f);
            GetComponent<SpriteRenderer>().color = new Color(col, col, col);
            yield return new WaitForSeconds(0.3f);
            GetComponent<SpriteRenderer>().color = new Color(col - 0.1f, col - 0.1f, col - 0.1f);
        }
    }

    public void FallInto() {
        //if(item == null)
    }

    public void FallOut() {
        if (item != null && type != SquareTypes.WIREBLOCK) {
            Square nextSquare = GetNeighborBottom();
            if (nextSquare != null) {
                if (nextSquare.IsNone()) {
                    for (int i = row + 1; i < LevelManager.THIS.maxRows; i++) {
                        if (LevelManager.THIS.GetSquare(col, i) != null) {
                            if (!LevelManager.THIS.GetSquare(col, i).IsNone()) {
                                nextSquare = LevelManager.THIS.GetSquare(col, i);
                                break;
                            }
                        }
                    }
                }
                if (nextSquare.CanFallInto()) {
                    if (nextSquare.item == null) {
                        item.CheckNeedToFall(nextSquare);
                    }
                }
            }
        }
    }

    public bool IsNone() {
        return type == SquareTypes.NONE;
    }

    public bool IsHaveDestroybleObstacle() {
        return type == SquareTypes.SOLIDBLOCK || type == SquareTypes.THRIVING;

    }

    public bool CanGoOut() {
        return type != SquareTypes.WIREBLOCK;
    }

    public bool CanGoInto() {
        return type != SquareTypes.SOLIDBLOCK && type != SquareTypes.UNDESTROYABLE && type != SquareTypes.NONE && type != SquareTypes.THRIVING;
    }

    public bool CanFallInto() {
        return type != SquareTypes.WIREBLOCK && type != SquareTypes.SOLIDBLOCK && type != SquareTypes.UNDESTROYABLE && type != SquareTypes.NONE && type != SquareTypes.THRIVING;
    }


    public void DestroyBlock() {
        if (type == SquareTypes.UNDESTROYABLE)
            return;
        if (type != SquareTypes.SOLIDBLOCK && type != SquareTypes.THRIVING) {
            List<Square> sqList = GetAllNeghbors();
            foreach (Square sq in sqList) {
                if (sq.type == SquareTypes.SOLIDBLOCK || sq.type == SquareTypes.THRIVING)
                    sq.DestroyBlock();
            }
        }
        if (block.Count > 0) {
            if (type == SquareTypes.BLOCK) {
                LevelManager.THIS.CheckCollectedTarget(block[block.Count - 1].gameObject);
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForBlock, transform.position, 0);
                LevelManager.THIS.TargetBlocks--;
                block[block.Count - 1].GetComponent<SpriteRenderer>().enabled = false;
            }
            if (type == SquareTypes.WIREBLOCK) {
                if (cageHP > 0)
                    return;
                LevelManager.THIS.TargetCages--;
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForWireBlock, transform.position, 0);
            }
            if (type == SquareTypes.SOLIDBLOCK) {
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForSolidBlock, transform.position, 0);
            }
            if (type == SquareTypes.THRIVING) {
                LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForThrivingBlock, transform.position, 0);
                //LevelManager.Instance.thrivingBlockDestroyed = true;
            }
            //Destroy( block[block.Count-1]);
            if (type != SquareTypes.BLOCK) {
                SoundBase.Instance.PlaySound(SoundBase.Instance.block_destroy);

                if (type != SquareTypes.THRIVING) {
                    block[block.Count - 1].GetComponent<Animation>().Play("BrickRotate");
                    block[block.Count - 1].GetComponent<SpriteRenderer>().sortingOrder = 4;
                    block[block.Count - 1].AddComponent<Rigidbody2D>();
                    block[block.Count - 1].GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(UnityEngine.Random.insideUnitCircle.x * UnityEngine.Random.Range(30, 200), UnityEngine.Random.Range(100, 150)), ForceMode2D.Force);
                }
                else {
                    block[block.Count - 1].SetActive(false);
                    GameObject boom = Instantiate(boomPrefab, transform.position, Quaternion.identity) as GameObject;
                    Destroy(boom, 1);
                }
            }
            GameObject.Destroy(block[block.Count - 1], 1.5f);
            //   if (block.Count > 1) type = SquareTypes.BLOCK;
            block.Remove(block[block.Count - 1]);
            if (block.Count > 0)
                type = block[block.Count - 1].GetComponent<Square>().type;

            if (block.Count == 0)
                type = SquareTypes.EMPTY;
        }

    }

    public void GenThriveBlock(Square newSquare) {

    }

    public List<Square> GetAllNeghbors() {
        List<Square> sqList = new List<Square>();
        Square nextSquare = null;
        nextSquare = GetNeighborBottom();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborTop();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborLeft();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        nextSquare = GetNeighborRight();
        if (nextSquare != null)
            sqList.Add(nextSquare);
        return sqList;
    }

    public bool IsHaveSolidAbove() {
        for (int i = row; i >= 0; i--) {
            if (LevelManager.THIS.GetSquare(col, i).type == SquareTypes.WIREBLOCK || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.SOLIDBLOCK || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.UNDESTROYABLE || LevelManager.THIS.GetSquare(col, i).type == SquareTypes.THRIVING)
                return true;
        }
        return false;
    }

    public void SetCage(int cageHP_) {
        cageHP = cageHP_;
        OldcageHP = cageHP;

    }

    bool justHighlighted;
    private bool canDamage;

    //estimate caculation of life if cage only highlighted

    public void SetActiveCage(bool active, bool _justHighlighted = false) {
        if (type == SquareTypes.WIREBLOCK) {
            justHighlighted = _justHighlighted;
            cageActive = active;
            if (active) {
                OldcageHP = cageHP;
            }

            //else if (cageHP < OldcageHP)
            //    cageHP = OldcageHP;

        }
    }


    //check is the cage going to be broken?
    public bool IsCageGoingToBroke(int extraDamage = 0) {
        if (type == SquareTypes.WIREBLOCK && cageHP >= 0) {
            int damage = LevelManager.THIS.destroyAnyway.Count + extraDamage;
            int estimateCageHP = Mathf.Clamp(cageHP - damage, 0, cageHP);
            OldcageHP = cageHP;
            AddDagame(damage);
            cageActive = false;

            //cageHP = Mathf.Clamp(OldcageHP - (LevelManager.THIS.destroyAnyway.Count + extraDamage), 0, OldcageHP);
            if (estimateCageHP == 0)
                return true;
            return false;
        }
        return true;
    }

    //check is cage will be destroy after damage
    public bool CheckDamage(int damage) {
        if (type == SquareTypes.WIREBLOCK && cageHP >= 0) {
            int estimateCageHP = Mathf.Clamp(cageHP - damage, 0, cageHP);
            canDamage = true;
            //cageHP = Mathf.Clamp(OldcageHP - damage, 0, OldcageHP);
            OldcageHP = cageHP;
            AddDagame(damage);
            if (estimateCageHP == 0)
                return true;
            return false;
        }
        return true;

    }

    public void AddDagame(int damage) {
        BurstIceParticles();
        //if (canDamage)
        //{
        canDamage = false;
        cageHP = OldcageHP - damage;
        if (cageHP < 0)
            cageHP = 0;
        //}
    }

    private void BurstIceParticles() {
        iTween.ShakePosition(block[block.Count - 1].gameObject, Vector3.one * 0.3f, 0.2f);
        GameObject ice = Instantiate(icePrefab, transform.position, Quaternion.identity) as GameObject;
        SoundBase.Instance.PlaySound(SoundBase.Instance.iceCrack);

        Destroy(ice, 1);
    }

    public bool IsExtraItem() {
        if (item != null) {
            if (item.IsExtraItem())
                return true;
        }
        return false;
    }
}
