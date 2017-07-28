using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public enum ItemsTypes
{
	NONE = 0,
	VERTICAL_STRIPPED,
	HORIZONTAL_STRIPPED,
	PACKAGE,
	CHOCOBOMB,
	INGREDIENT,
	BOMB
}

public class Item : MonoBehaviour
{
	public Sprite[] items;
	public Sprite[] itemsAnimation;
	public List<StripedItem> stripedItems = new List<StripedItem> ();
	public Sprite[] packageItems;
	public Sprite[] ChocoBombItems;
	public Sprite[] bombItems;
	Sprite[] ingredientItems;
	public SpriteRenderer sprRenderer;
	public Square square;
	public bool dragThis;
	public Vector3 mousePos;
	public Vector3 deltaPos;
	public Vector3 switchDirection;
	private Square neighborSquare;
	private Item switchItem;
	public bool falling;
	ItemsTypes NextType = ItemsTypes.NONE;

	public ItemsTypes nextType {
		get {
			return NextType;
		}
		set {
			NextType = value;
		}
	}

	public ItemsTypes CurrentType = ItemsTypes.NONE;

	public ItemsTypes currentType {
		get {
			return CurrentType;
		}
		set {

			CurrentType = value;
		}
	}

	public ItemsTypes debugType = ItemsTypes.NONE;
	public int COLORView;
	private int COLOR;

	public int color {
		get {
			return COLOR;
		}
		set {
			COLOR = value;
		}
	}

	public Animator anim;
	public bool destroying;
	public bool appeared;
	public bool animationFinished;
	public bool justCreatedItem;
	private float xScale;
	private float yScale;
	public GameObject timerTextPrefab;
	Text timerText;
	public int bombTimer;
	public GameObject bombSelection;
	public GameObject bombSelection1;
	GameObject light;
	public GameObject wickBurningPrefab;
	public bool awaken;
	public GameObject appearingEffect;
	private bool extraChecked;
	public Item item;

	// Use this for initialization
	void Start ()
	{
		item = this;
		falling = true;
		ingredientItems = LevelManager.THIS.ingrediendSprites;
		GenColor ();
		// sprRenderer = GetComponentInChildren<SpriteRenderer>();
		if (nextType != ItemsTypes.NONE) {
			debugType = nextType;
			currentType = nextType;
			nextType = ItemsTypes.NONE;
			transform.position = square.transform.position;
			falling = false;
		}
		// else if (LevelManager.THIS.limitType == LIMIT.TIME && UnityEngine.Random.Range(0, 28) == 1)
		// {
		//     GameObject fiveTimes = Instantiate(Resources.Load("Prefabs/5sec")) as GameObject;
		//     fiveTimes.transform.SetParent(transform);
		//     fiveTimes.name = "5sec";
		//     fiveTimes.transform.localScale = Vector3.one * 2;
		//     fiveTimes.transform.localPosition = Vector3.zero;
		// }
		xScale = transform.localScale.x;
		yScale = transform.localScale.y;
		if (currentType == ItemsTypes.INGREDIENT) {
			GameObject obj = Instantiate (Resources.Load ("Prefabs/arrow_ingredients")) as GameObject;
			obj.transform.SetParent (transform);
			obj.transform.localPosition = new Vector3 (0.66f, -0.53f, 0);
			obj.transform.localScale = Vector3.one * 0.6f;

		}
		//StartCoroutine(GenRandomSprite());
	}

	public void GenColor (int exceptColor = -1, bool onlyNONEType = false)
	{
		int row = square.row;
		int col = square.col;

		List<int> remainColors = new List<int> ();
		for (int i = 0; i < LevelManager.Instance.colorLimit; i++) {
			bool canGen = true;
			if (col > 1) {
				Square neighbor = LevelManager.Instance.GetSquare (row, col - 1);
				if (neighbor != null) {
					if (neighbor.item != null) {
						if (neighbor.CanGoInto () && neighbor.item.color == i)
							canGen = false;
					}
				}
			}
			if (col < LevelManager.Instance.maxCols - 1) {
				Square neighbor = LevelManager.Instance.GetSquare (row, col + 1);
				if (neighbor != null) {
					if (neighbor.item != null) {
						if (neighbor.CanGoOut () && neighbor.item.color == i)
							canGen = false;
					}
				}
			}
			if (row < LevelManager.Instance.maxRows) {
				Square neighbor = LevelManager.Instance.GetSquare (row + 1, col);
				if (neighbor != null) {
					if (neighbor.item != null) {
						if (neighbor.CanGoOut () && neighbor.item.color == i)
							canGen = false;
					}
				}
			}
			if (canGen && i != exceptColor) {
				remainColors.Add (i);
			}
		}

		//       print(remainColors.Count);
		int randColor = UnityEngine.Random.Range (0, LevelManager.Instance.colorLimit);
		if (remainColors.Count > 0)
			randColor = remainColors [UnityEngine.Random.Range (0, remainColors.Count)];
		if (exceptColor == randColor)
			randColor = (randColor++) % items.Length;
		LevelManager.THIS.lastRandColor = randColor;
		sprRenderer.sprite = items [randColor];
		if (nextType == ItemsTypes.HORIZONTAL_STRIPPED)
			sprRenderer.sprite = stripedItems [color].horizontal;
		else if (nextType == ItemsTypes.VERTICAL_STRIPPED)
			sprRenderer.sprite = stripedItems [color].vertical;
		else if (nextType == ItemsTypes.PACKAGE)
			sprRenderer.sprite = packageItems [color];
		else if (nextType == ItemsTypes.CHOCOBOMB)
			sprRenderer.sprite = ChocoBombItems [0];
		else if (LevelManager.THIS.target == Target.COLLECT) {

			for (int i = 0; i < LevelManager.THIS.NumIngredients; i++) {

				if (UnityEngine.Random.Range (0, 10 + (10 * LevelManager.THIS.GetIngredients ().Count)) == 0 && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType && LevelManager.THIS.ingrTarget [i].count > 0) {
					if (LevelManager.THIS.GetIngredients (i).Count < LevelManager.THIS.ingrTarget [i].count) {
						SetIngr (i);
						break;
					} else {

						StartCoroutine (FallingCor (square, true));
						color = Array.IndexOf (items, sprRenderer.sprite);
					}

				} else {

					StartCoroutine (FallingCor (square, true));
					color = Array.IndexOf (items, sprRenderer.sprite);
				}
			}
		} else {
			StartCoroutine (FallingCor (square, true));
			color = Array.IndexOf (items, sprRenderer.sprite);
		}
	}

	void SetIngr (int i)
	{
		StartCoroutine (FallingCor (square, true));
		color = 1000 + (int)i;
		currentType = ItemsTypes.INGREDIENT;
		sprRenderer.sprite = LevelManager.THIS.ingrTarget [i].sprite;
	}

	public void AwakeItem ()
	{
		awaken = true;
		anim.SetTrigger ("Idle");

		if (currentType != ItemsTypes.BOMB)
			sprRenderer.sprite = itemsAnimation [color];
		else if (currentType == ItemsTypes.BOMB) {
			SetLight ();
		}
	}

	public void SetLight ()
	{
		if (light == null) {
			light = Instantiate (bombSelection) as GameObject;
			light.transform.SetParent (transform);
			light.name = "BombSelection";
			light.transform.localPosition = Vector3.zero;
			if (currentType == ItemsTypes.BOMB)
				light.transform.localScale = Vector3.one * 3.6f;
			else
				light.transform.localScale = Vector3.one * 2f;
		}
		light.SetActive (true);

	}

	public void SleepItem ()
	{
		awaken = false;
		if (!anim)
			return;//1.3
		anim.SetTrigger ("IdleStop");

		if (currentType != ItemsTypes.BOMB)
			sprRenderer.sprite = items [color];
		if (light != null) {
			light.SetActive (false);

		}

	}


	public void SetColor (int col)
	{
		color = col;
		sprRenderer.sprite = items [color];
	}

	public void SetAppeared ()
	{
		appeared = true;
		if (nextType != ItemsTypes.BOMB)
			StartIdleAnim ();
		if (currentType == ItemsTypes.PACKAGE)
			anim.SetBool ("package_idle", true);

	}

	public void StartIdleAnim ()
	{
		StartCoroutine (AnimIdleStart ());

	}

	IEnumerator AnimIdleStart ()
	{
		float xScaleDest1 = xScale - 0.05f;
		float xScaleDest2 = xScale;
		float speed = UnityEngine.Random.Range (0.02f, 0.07f);

		bool trigger = false;
		while (true) {
			if (!trigger) {
				if (xScale > xScaleDest1) {
					xScale -= Time.deltaTime * speed;
					yScale += Time.deltaTime * speed;
				} else
					trigger = true;
			} else {
				if (xScale < xScaleDest2) {
					xScale += Time.deltaTime * speed;
					yScale -= Time.deltaTime * speed;
				} else
					trigger = false;
			}
			transform.localScale = new Vector3 (xScale, yScale, 1);
			yield return new WaitForFixedUpdate ();
		}
		//anim.SetBool("stop", true);
		//if (square.col % 2 == 0 || square.row % 2 == 0)
		//    yield return new WaitForSeconds(1);
		//anim.SetBool("stop", false);
	}

	IEnumerator GenRandomSprite ()
	{
		Sprite spr = null;
		while (true) {
			spr = items [UnityEngine.Random.Range (0, items.Length)];
			yield return new WaitForFixedUpdate ();
			break;
		}

		sprRenderer.sprite = spr;
	}

	//void OnMouseDown()
	//{
	//    if (!LevelManager.THIS.DragBlocked && LevelManager.THIS.gameStatus == GameState.Playing)
	//    {
	//        if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb && currentType != ItemsTypes.BOMB && currentType != ItemsTypes.INGREDIENT)
	//        {
	//            SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.boostBomb);
	//            LevelManager.THIS.DragBlocked = true;
	//            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/bomb"), transform.position, transform.rotation) as GameObject;
	//            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
	//            obj.GetComponent<BoostAnimation>().square = square;
	//            LevelManager.THIS.ActivatedBoost = null;
	//        }
	//        else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Random_color && currentType != ItemsTypes.BOMB)
	//        {
	//            SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.boostColorReplace);
	//            LevelManager.THIS.DragBlocked = true;
	//            GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/random_color_item"), transform.position, transform.rotation) as GameObject;
	//            obj.GetComponent<BoostAnimation>().square = square;
	//            obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
	//            LevelManager.THIS.ActivatedBoost = null;
	//        }
	//        else if(square.type != SquareTypes.WIREBLOCK)
	//        {
	//            dragThis = true;
	//            mousePos = GetMousePosition();
	//            deltaPos = Vector3.zero;
	//        }
	//    }
	//}


	//void OnMouseUp()
	//{
	//    dragThis = false;
	//    switchDirection = Vector3.zero;
	//}

	void ResetDrag ()
	{
		dragThis = false;
		//   transform.position = square.transform.position + Vector3.back * 0.2f;
		switchDirection = Vector3.zero;
		//    switchItem.transform.position = neighborSquare.transform.position + Vector3.back * 0.2f;
		neighborSquare = null;
		switchItem = null;

	}

	void Update ()
	{
		COLORView = color;
		if (currentType != debugType && currentType != ItemsTypes.INGREDIENT && LevelManager.THIS.gameStatus == GameState.Playing) {
			print ("debug type " + debugType);
			print ("current type " + currentType);
			nextType = debugType;
			ChangeType ();

		}


		if (currentType == ItemsTypes.BOMB && timerText != null)
			timerText.text = "" + bombTimer;

	}

	public void CheckChocoBomb (Item item1, Item item2)
	{
		if (item1.currentType == ItemsTypes.INGREDIENT || item2.currentType == ItemsTypes.INGREDIENT)
			return;
		if (item1.currentType == ItemsTypes.CHOCOBOMB) {
			if (item2.currentType == ItemsTypes.NONE)
				DestroyColor (item2.color);
			else if (item2.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item2.currentType == ItemsTypes.VERTICAL_STRIPPED)
				LevelManager.THIS.SetTypeByColor (item2.color, ItemsTypes.HORIZONTAL_STRIPPED);
			else if (item2.currentType == ItemsTypes.PACKAGE)
				LevelManager.THIS.SetTypeByColor (item2.color, ItemsTypes.PACKAGE);
			else if (item2.currentType == ItemsTypes.CHOCOBOMB)
				LevelManager.THIS.DestroyDoubleBomb (square.col);



			item1.DestroyItem ();
		}

	}

	public void CheckNeedToFall (Square _square)
	{
		_square.item = this;
		square.item = null;
		square = _square;   //need to count all falling items and drop them down in the same time
	}

	public void StartFalling ()
	{
		if (!falling)
			StartCoroutine (FallingCor (square, true));
	}

	IEnumerator FallingCor (Square _square, bool animate)
	{
		falling = true;
		float startTime = Time.time;
		Vector3 startPos = transform.position;
		float speed = 10;
		if (LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
			speed = 10;
		float distance = Vector3.Distance (startPos, _square.transform.position);
		float fracJourney = 0;
		if (distance > 0.5f) {
			while (fracJourney < 1) {
				speed += 0.2f;
				float distCovered = (Time.time - startTime) * speed;
				fracJourney = distCovered / distance;
				transform.position = Vector3.Lerp (startPos, _square.transform.position + Vector3.back * 0.2f, fracJourney);
				yield return new WaitForFixedUpdate ();
				if (fracJourney > 0.3f)
					falling = false;

			}
		}
		if (distance > 0.5f && animate) {
			anim.SetTrigger ("stop");
			SoundBase.Instance.PlaySound (SoundBase.Instance.drop [UnityEngine.Random.Range (0, SoundBase.Instance.drop.Length)]);
		}
		falling = false;
		justCreatedItem = false;
	}

	public bool GetNearEmptySquares ()
	{
		bool nearEmptySquareDetected = false;
		if (!square.CanGoOut ())
			return false;
		if (square.row < LevelManager.Instance.maxRows - 1 && square.col < LevelManager.Instance.maxCols) {
			Square checkingSquare = LevelManager.Instance.GetSquare (square.col + 1, square.row + 1, true);
			if (checkingSquare.CanFallInto () && checkingSquare.item == null && !falling) {//1.3.2
				checkingSquare = LevelManager.Instance.GetSquare (square.col + 1, square.row + 1, true);
				if (checkingSquare.CanFallInto ()) {
					if (checkingSquare.item == null) {
						square.item = null;
						checkingSquare.item = this;
						square = checkingSquare;
						StartFalling ();//1.3.2
						nearEmptySquareDetected = true;
					}
				}
			}
		}
		if (square.row < LevelManager.Instance.maxRows - 1 && square.col > 0) {
			Square checkingSquare = LevelManager.Instance.GetSquare (square.col - 1, square.row + 1, true);
			if (checkingSquare.CanFallInto () && checkingSquare.item == null && !falling) {//1.3.2
				checkingSquare = LevelManager.Instance.GetSquare (square.col - 1, square.row + 1, true);
				if (checkingSquare.CanFallInto ()) {
					if (checkingSquare.item == null) {
						square.item = null;
						checkingSquare.item = this;
						square = checkingSquare;
						StartFalling ();//1.3.2
						nearEmptySquareDetected = true;
					}
				}
			}
		}
		return nearEmptySquareDetected;
	}

	void StripeEffect (ItemsTypes _itemType)
	{
		GameObject obj = Instantiate (Resources.Load ("Prefabs/StripeEffect")) as GameObject;
		obj.transform.SetParent (transform.Find ("Sprite"));
		obj.transform.localScale = Vector3.one;
		obj.GetComponent<StripesWrappEffect> ().itemSprite = transform.Find ("Sprite").GetComponent<SpriteRenderer> ();
		obj.transform.localPosition = Vector3.zero;
		if (_itemType == ItemsTypes.HORIZONTAL_STRIPPED)
			obj.transform.eulerAngles = new Vector3 (0, 0, 90);
		transform.Find ("Sprite").GetComponent<SpriteRenderer> ().sortingOrder = 3;

		GameObject effect = Instantiate (appearingEffect) as GameObject;
		effect.transform.position = transform.position;
		Destroy (effect, 2);



	}

	public void ChangeType ()
	{
		if (this != null)
			StartCoroutine (ChangeTypeCor ());
	}

	IEnumerator ChangeTypeCor ()
	{
		if (nextType == ItemsTypes.HORIZONTAL_STRIPPED) {
			StripeEffect (nextType);
			anim.SetTrigger ("appear");
			SoundBase.Instance.PlaySound (SoundBase.Instance.appearStipedColorBomb);
		} else if (nextType == ItemsTypes.VERTICAL_STRIPPED) {
			StripeEffect (nextType);
			anim.SetTrigger ("appear");
			SoundBase.Instance.PlaySound (SoundBase.Instance.appearStipedColorBomb);
		} else if (nextType == ItemsTypes.PACKAGE) {
			anim.SetTrigger ("appear");
			SoundBase.Instance.PlaySound (SoundBase.Instance.appearStipedColorBomb);

		} else if (nextType == ItemsTypes.CHOCOBOMB) {
			anim.SetTrigger ("appear");
			SoundBase.Instance.PlaySound (SoundBase.Instance.appearStipedColorBomb);
			color = 555;
		} else if (nextType == ItemsTypes.BOMB) {
			anim.SetTrigger ("appear");
			SoundBase.Instance.PlaySound (SoundBase.Instance.appearStipedColorBomb);
			xScale /= 2f;
			yScale /= 2f;
			transform.localScale /= 1.5f;
		}

		while (!appeared)
			yield return new WaitForFixedUpdate ();

		//   sprRenderer.transform.localScale = Vector3.one;
		if (nextType == ItemsTypes.NONE)
			yield break;
		// sprRenderer.enabled = true;
		// if (nextType == ItemsTypes.HORIZONTAL_STRIPPED)
		//     sprRenderer.sprite = stripedItems[color].horizontal;
		// else if (nextType == ItemsTypes.VERTICAL_STRIPPED)
		//     sprRenderer.sprite = stripedItems[color].vertical;
		if (nextType == ItemsTypes.PACKAGE)
			sprRenderer.sprite = packageItems [color];
		else if (nextType == ItemsTypes.CHOCOBOMB)
			sprRenderer.sprite = ChocoBombItems [0];
		else if (nextType == ItemsTypes.BOMB) {
			sprRenderer.sprite = bombItems [color];
			SetupBomb ();
		}

		//     square.DestroyBlock();

		debugType = nextType;
		currentType = nextType;
		nextType = ItemsTypes.NONE;

	}

	void SetupBomb ()
	{

		anim.SetBool ("package_idle", true);

		GameObject t = Instantiate (timerTextPrefab) as GameObject;
		t.transform.SetParent (transform);
		t.transform.localPosition = new Vector3 (1.5f, -0.8f, 0);
		t.transform.localScale = Vector3.one;
		timerText = t.transform.GetChild (0).GetComponent<Text> ();
		if (bombTimer <= 0)//1.3
			bombTimer = LevelManager.Instance.bombTimer;

		GameObject wickBurning = Instantiate (wickBurningPrefab) as GameObject;
		wickBurning.transform.SetParent (transform);
		wickBurning.transform.localPosition = new Vector3 (0.69f, 0.85f, 0);
		wickBurning.transform.localScale = Vector3.one * 0.2f;
	}

	public void BombTick ()
	{
		bombTimer--;
		if (bombTimer <= 0 && currentType == ItemsTypes.BOMB && LevelManager.Instance.gameStatus == GameState.Playing) {
			LevelManager.Instance.gameStatus = GameState.PreFailedBomb;
			AwakeItem ();
			sprRenderer.sortingOrder = 4;
			timerText.enabled = false;
			square = null;
			AnimatorHelper.Instance.BombFailed (gameObject, transform.position, Vector3.zero, 20, BombFailedAnimCallBack);
		}
	}

	public void BombFailedAnimCallBack ()
	{
		LevelManager.Instance.gameStatus = GameState.GameOver;
	}


	public void SetAnimationDestroyingFinished ()
	{
		LevelManager.THIS.itemsHided = true;
		animationFinished = true;
	}

	#region Destroying

	public void DestroyItem (bool showScore = false, string anim_name = "", bool explEffect = false, bool directly = false)
	{
		if (destroying)
			return;
		// if (nextType != ItemsTypes.NONE) return;
		if (this == null)
			return;
		StopCoroutine (AnimIdleStart ());
		destroying = true;
		square.item = null;

		if (this == null)
			return;

		StartCoroutine (DestroyCor (showScore, anim_name, explEffect, directly));


	}

	public void SetHighlight (ItemsTypes thisType)
	{
		List<Item> itemsList = new List<Item> ();
		if (thisType == ItemsTypes.HORIZONTAL_STRIPPED)
			itemsList = LevelManager.THIS.GetRow (square.row);
		else if (thisType == ItemsTypes.VERTICAL_STRIPPED)
			itemsList = LevelManager.THIS.GetColumn (square.col);

		foreach (Item itemSelected in itemsList) {
			if (itemSelected != null) {
				if ((itemSelected.currentType == ItemsTypes.HORIZONTAL_STRIPPED || itemSelected.currentType == ItemsTypes.VERTICAL_STRIPPED) && LevelManager.THIS.highlightedItems.IndexOf (itemSelected) < 0 && itemSelected != this) {
					LevelManager.THIS.highlightedItems.Add (itemSelected);
					ItemsTypes reverseType = ItemsTypes.HORIZONTAL_STRIPPED;
					if (thisType == ItemsTypes.HORIZONTAL_STRIPPED)
						reverseType = ItemsTypes.VERTICAL_STRIPPED;
					itemSelected.SetHighlight (reverseType);
				}

				itemSelected.square.SetActiveCage (true, true);
				itemSelected.square.HighLight (true);
			}
		}

	}

	IEnumerator DestroyCor (bool showScore = false, string anim_name = "", bool explEffect = false, bool directly = false)
	{
		//if (anim_name == "")
		//{
		anim.SetTrigger ("IdleStop");

		if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
			PlayDestroyAnimation ("destroy");
		else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
			PlayDestroyAnimation ("destroy");
		else if (currentType == ItemsTypes.PACKAGE) {
			PlayDestroyAnimation ("destroy");
			yield return new WaitForSeconds (0.1f);

			GameObject partcl = Instantiate (Resources.Load ("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
			partcl.GetComponent<ParticleSystem> ().startColor = LevelManager.THIS.scoresColors [color];
			Destroy (partcl, 1f);
		}
		if (currentType != ItemsTypes.INGREDIENT && currentType != ItemsTypes.CHOCOBOMB) {
			if (square.type == SquareTypes.WIREBLOCK) {
				if (square.cageHP > 0) {
					destroying = false;
					square.item = this;
					yield break;
				}
			}

			PlayDestroyAnimation ("destroy");

			GameObject partcl = LevelManager.THIS.GetExplFromPool ();
			if (partcl != null) {
				partcl.GetComponent<ItemAnimEvents> ().item = this;
				partcl.transform.localScale = Vector3.one * 1f;
				partcl.transform.position = transform.position;// + (Vector3)UnityEngine.Random.insideUnitCircle / 3;
				GameObject psObj = Instantiate (partcl.GetComponent<ItemAnimEvents> ().particals, partcl.transform.position, Quaternion.identity) as GameObject;
				int spr = 0;
				if (color == 0)
					spr = 3;
				if (color == 1)
					spr = 2;
				if (color == 2)
					spr = 4;
				if (color == 3)
					spr = 1;
				if (color == 4)
					spr = 0;
				if (color == 5)
					spr = 5;

				var ps = psObj.GetComponent<ParticleSystem> ();
				var ts = ps.textureSheetAnimation;
				ts.frameOverTime = new ParticleSystem.MinMaxCurve ((float)spr / ts.numTilesX);

				psObj.GetComponent<ParticleSystem> ().Play ();
				Destroy (psObj, 2);
				partcl.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, UnityEngine.Random.Range (0f, 360f)));
				partcl.GetComponent<Animator> ().SetInteger ("color", color);
				SoundBase.Instance.PlaySoundsRandom (SoundBase.Instance.pops);
				//SoundBase.Instance.PlaySound(SoundBase.Instance.destroy[UnityEngine.Random.Range(0, SoundBase.Instance.destroy.Length)]);
				//   Destroy(partcl, 1f);
			}
		}

		if (LevelManager.THIS.limitType == LIMIT.TIME && transform.Find ("5sec") != null) {
			GameObject FiveSec = transform.Find ("5sec").gameObject;
			FiveSec.transform.SetParent (null);
#if UNITY_5
			FiveSec.GetComponent<Animation> ().clip.legacy = true;
#endif

			FiveSec.GetComponent<Animation> ().Play ("5secfly");
			Destroy (FiveSec, 1);
			if (LevelManager.THIS.gameStatus == GameState.Playing)
				LevelManager.THIS.Limit += 5;
		}

		//Color color = sprRenderer.sprite.texture.GetPixel(sprRenderer.sprite.texture.width / 2 - 10, sprRenderer.sprite.texture.height / 2 - 10);
		if (showScore)
			LevelManager.THIS.PopupScore (LevelManager.THIS.scoreForItem, transform.position, color);

		LevelManager.THIS.CheckCollectedTarget (gameObject);

		while (!animationFinished && currentType == ItemsTypes.NONE)
			yield return new WaitForFixedUpdate ();
		//if (directly)
		//    square.AddDagame(1);
		//else
		//    square.AddDagame(5);

		square.DestroyBlock ();
		if (directly) {
			if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
				DestroyHorizontal ();
			else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
				DestroyVertical ();
		}
		if (currentType == ItemsTypes.PACKAGE)
			DestroyPackage ();
		else if (currentType == ItemsTypes.CHOCOBOMB && LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
			CheckChocoBomb (this, LevelManager.THIS.GetRandomItems (1) [0]);

		if (nextType != ItemsTypes.NONE) {
			Item i = square.GenItem ();
			i.nextType = nextType;
			i.SetColor (color);
			i.ChangeType ();
		}
		if (destroying) {
			Destroy (gameObject);
		}
	}

	//void OnGUI() {
	//    GUILayout.TextArea("" + xxx);
	//}

	public void DestroyHorizontal (bool boost = false)
	{
		StartCoroutine (DestroyStrippedCor (boost, true));
	}

	public void DestroyVertical (bool boost = false)
	{
		StartCoroutine (DestroyStrippedCor (boost, false));
	}


	IEnumerator DestroyStrippedCor (bool boost, bool horrizontal)
	{
		SoundBase.Instance.PlayLimitSound (SoundBase.Instance.strippedExplosion);
		LevelManager.THIS.StrippedShow (gameObject, horrizontal);
		List<Square> itemsList = null;
		if (horrizontal)
			itemsList = LevelManager.THIS.GetRowSquare (square.row);
		else
			itemsList = LevelManager.THIS.GetColumnSquare (square.col);
		foreach (var _square in itemsList) {
			if (_square != null) {
				if (_square.item != this && _square.item != null) {
					if (_square.item.currentType != ItemsTypes.CHOCOBOMB && _square.item.currentType != ItemsTypes.INGREDIENT && _square.square.CheckDamage (5) && !_square.item.extraChecked) {
						if (_square.item.currentType == ItemsTypes.HORIZONTAL_STRIPPED) {
							_square.item.extraChecked = true;
							_square.item.DestroyVertical ();
						} else if (_square.item.currentType == ItemsTypes.VERTICAL_STRIPPED) {
							_square.item.extraChecked = true;
							_square.item.DestroyHorizontal ();
						} else {
							_square.item.DestroyItem (true, "", false, true);
						}
					}

				} else if (_square.item == null && (_square.square.type == SquareTypes.BLOCK || _square.square.type == SquareTypes.SOLIDBLOCK || _square.square.type == SquareTypes.THRIVING)) {
					_square.square.DestroyBlock ();
				}
			}
		}
		yield return new WaitForFixedUpdate ();

		//List<Square> sqList = null;
		//if (horrizontal)
		//    sqList = LevelManager.THIS.GetRowSquaresObstacles(square.row);
		//else
		//    sqList = LevelManager.THIS.GetColumnSquaresObstacles(square.col);
		//foreach (Square item in sqList)
		//{
		//    if (item != null)
		//        item.DestroyBlock();
		//}
		if (!boost)
			DestroyItem (true, "", false, true);

	}



	public void DestroyPackage ()
	{
		SoundBase.Instance.PlaySound (SoundBase.Instance.destroyPackage);

		List<Item> itemsList = LevelManager.THIS.GetItemsAround (square);
		foreach (Item item in itemsList) {
			if (item != null)
			if (item != null) {
				if (item.currentType != ItemsTypes.CHOCOBOMB && item.currentType != ItemsTypes.INGREDIENT)
					item.DestroyItem (true, "destroy_package");
			}
		}
		SoundBase.Instance.PlaySound (SoundBase.Instance.explosion);
		currentType = ItemsTypes.NONE;
		DestroyItem (true);
	}

	public void DestroyColor (int p)
	{
		SoundBase.Instance.PlaySound (SoundBase.Instance.colorBombExpl);

		GameObject[] items = GameObject.FindGameObjectsWithTag ("Item");
		foreach (GameObject item in items) {
			if (item.GetComponent<Item> ().color == p)
				item.GetComponent<Item> ().DestroyItem (true, "", true);
		}
	}

	void PlayDestroyAnimation (string anim_name)
	{
		anim.SetTrigger (anim_name);

	}

	public void SmoothDestroy ()
	{
		StartCoroutine (SmoothDestroyCor ());
	}

	IEnumerator SmoothDestroyCor ()
	{
		square.item = null;
		anim.SetTrigger ("disAppear");
		yield return new WaitForSeconds (1);
		Destroy (gameObject);
	}

	#endregion

	public bool IsExtraItem ()
	{
		return currentType == ItemsTypes.HORIZONTAL_STRIPPED || currentType == ItemsTypes.VERTICAL_STRIPPED;
	}

}

[System.Serializable]
public class StripedItem
{
	public Sprite horizontal;
	public Sprite vertical;
}
