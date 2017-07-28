using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cage : MonoBehaviour
{
    public Text HP;
    private Square square;

    // Use this for initialization
    void Start()
    {
        square = transform.parent.GetComponent<Square>();
    }

    // Update is called once per frame
    void Update()
    {
        HP.text = "" + square.cageHPPreview;
    }
}
