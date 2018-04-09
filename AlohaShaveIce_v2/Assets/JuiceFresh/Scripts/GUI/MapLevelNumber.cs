using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapLevelNumber : MonoBehaviour {
	[SerializeField]
	Text levelText;
	// Use this for initialization
//	void Start () {
//        //renderer.sortingLayerID = 0;
//        //renderer.sortingOrder = 2;
////		int num = int.Parse( transform.parent.parent.parent.name.Replace( "Level", "" ) );
//        //GetComponent<TextMesh>().text = "" + num;
////		GetComponent<Text>().text = mapLevel.Number.ToString();
////		SetLevel(mapLevel.Number);
//      //  if( num >= 10 ) transform.position += Vector3.left * 0.05f;
//   //     if( num == 1 || num == 11 ) transform.position -= Vector3.right * 0.05f;
//	}

	public void SetLevel (int _level)
	{
		levelText.text = _level.ToString();
	}

}
