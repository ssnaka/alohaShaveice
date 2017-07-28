using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{
    Image slider;
    public static ProgressBarScript Instance;
    float maxWidth;
    public GameObject[] stars;

    public Vector3 pivot;
    public Vector3 angles;
    // Use this for initialization
    void OnEnable()
    {
        Instance = this;
        slider = GetComponent<Image>();
        maxWidth = 1f;
        ResetBar();
        PrepareStars();
    }

    public void UpdateDisplay(float x)
    {
        slider.fillAmount = maxWidth - maxWidth * x;
        if (maxWidth - maxWidth * x < 0)
        {
            slider.fillAmount = 0;

            //	ResetBar();
        }
    }

    public void AddValue(float x)
    {
        UpdateDisplay(slider.fillAmount * 100 / maxWidth / 100 + x);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsFull()
    {
        if (slider.fillAmount >= maxWidth)
        {
            ResetBar();
            return true;
        }
        else
            return false;
    }

    public void ResetBar()
    {
        UpdateDisplay(0.0f);
    }

    void PrepareStars()
    {
        if (LevelManager.THIS != null)
        {

            float width = 532;

            float angleStar1 = LevelManager.Instance.star1 * (width / 2f) / LevelManager.Instance.star3 * width / 100 - (width / 2f) + 90;
            float angleStar2 = LevelManager.Instance.star2 * (width / 2f) / LevelManager.Instance.star3 * width / 100 - (width / 2f) + 10;
            //print(((float)LevelManager.Instance.star1 * 100f / (float)LevelManager.Instance.star3  ));
            //print(angleStar2);
            stars[0].transform.localPosition = new Vector3(LevelManager.Instance.star1 * 100 / LevelManager.Instance.star3 * width / 100 - (width / 2f), stars[0].transform.localPosition.y, 0);
            stars[1].transform.localPosition = new Vector3(LevelManager.Instance.star2 * 100 / LevelManager.Instance.star3 * width / 100 - (width / 2f), stars[1].transform.localPosition.y, 0);
            //stars[0].transform.localPosition = RotatePointAroundPivot(stars[0].transform.localPosition, pivot, new Vector3(0, 0, angleStar1));
            //stars[1].transform.localPosition = RotatePointAroundPivot(stars[1].transform.localPosition, pivot, new Vector3(0, 0, angleStar2));
            stars[0].transform.GetChild(0).gameObject.SetActive(false);
            stars[1].transform.GetChild(0).gameObject.SetActive(false);
            stars[2].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

}
