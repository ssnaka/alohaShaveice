using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovesTime : MonoBehaviour
{
    public Sprite[] sprites;
    // Use this for initialization
    void OnEnable()
    {
        StartCoroutine(WaitForLoading());
    }

    IEnumerator WaitForLoading()
    {
        yield return LevelManager.THIS;
        if (LevelManager.THIS.limitType == LIMIT.MOVES)
            GetComponent<Image>().sprite = sprites[0];
        else
            GetComponent<Image>().sprite = sprites[1];
    }

    // Update is called once per frame
    void Update()
    {

    }
}
