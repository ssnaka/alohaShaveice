using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoostIcon : MonoBehaviour
{
    public Text boostCount;
    public BoostType type;
    bool check;
    public Text price;

	public bool canUseBoost = true;

    void OnEnable()
    {
        if (name != "Main Camera")
        {
            if (LevelManager.THIS != null)
            {
                if (LevelManager.THIS.gameStatus == GameState.Map)
                    transform.Find("Indicator/Count/Check").gameObject.SetActive(false);
                if (!LevelManager.THIS.enableInApps)
                    gameObject.SetActive(false);
            }

			if (!canUseBoost)
			{
				transform.Find("Indicator/Count/Check").gameObject.SetActive(false);
			}
        }

		Messenger.AddListener<BoostType, int>("BoostValueChanged", OnBoostValueChanged);
    }

	void Start ()
	{
		Messenger.AddListener<BoostType, int>("BoostValueChanged", OnBoostValueChanged);

		OnBoostValueChanged(type, PlayerPrefs.GetInt("" + type));

	}

	void OnDisable ()
	{
		Messenger.RemoveListener<BoostType, int>("BoostValueChanged", OnBoostValueChanged);
	}


    public void ActivateBoost()
    {
		if (canUseBoost)
		{
	        if (LevelManager.THIS.ActivatedBoost == this)
	        {
	            UnCheckBoost();
	            return;
	        }
	        if (IsLocked() || check || (LevelManager.THIS.gameStatus != GameState.Playing && LevelManager.THIS.gameStatus != GameState.Map))
	            return;
	        if (BoostCount() > 0)
	        {
	            if (type != BoostType.Colorful_bomb && type != BoostType.Stripes && !LevelManager.THIS.DragBlocked)
	                LevelManager.THIS.ActivatedBoost = this;
	            if (type == BoostType.Colorful_bomb)
	            {
	                LevelManager.THIS.BoostColorfullBomb = 1;
	                Check();
	            }
	            if (type == BoostType.Stripes)
	            {
	                LevelManager.THIS.BoostStriped = 2;
	                Check();
	            }

	        }
	        else
	        {
	            OpenBoostShop(type);
	        }
		}
		else
		{
			OpenBoostShop(type);
		}
    }


    void UnCheckBoost()
    {
        LevelManager.THIS.activatedBoost = null;
        LevelManager.THIS.UnLockBoosts();
    }

    public void InitBoost()
    {
        transform.Find("Indicator/Count/Check").gameObject.SetActive(false);
        transform.Find("Indicator/Count/Count").gameObject.SetActive(true);
        LevelManager.THIS.BoostColorfullBomb = 0;
        LevelManager.THIS.BoostPackage = 0;
        LevelManager.THIS.BoostStriped = 0;
        check = false;
    }

    void Check()
    {
        check = true;
        transform.Find("Indicator/Count/Check").gameObject.SetActive(true);
        transform.Find("Indicator/Count/Count").gameObject.SetActive(false);
        //InitScript.Instance.SpendBoost(type);
    }

    public void LockBoost()
    {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(c.a, c.g, c.b, 0.5f);
        //transform.Find("Lock").gameObject.SetActive(true);
        transform.Find("Indicator").gameObject.SetActive(false);
    }

    public void UnLockBoost()
    {
        Color c = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(1, 1, 1, 1);

        //transform.Find("Lock").gameObject.SetActive(false);
        transform.Find("Indicator").gameObject.SetActive(true);
    }

    bool IsLocked()
    {
        return false;
    }

    int BoostCount()
    {
        return int.Parse(boostCount.text);
    }

    public void OpenBoostShop(BoostType boosType)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        GameObject.Find("CanvasGlobal").transform.Find("BoostShop").gameObject.GetComponent<BoostShop>().SetBoost(boosType);
    }

    void ShowPlus(bool show)
    {
        transform.Find("Indicator/Plus").gameObject.SetActive(show);
        transform.Find("Indicator/Count").gameObject.SetActive(!show);
    }

	void OnBoostValueChanged (BoostType _type, int _count)
	{
		if (boostCount != null && type.Equals(_type))
		{
			boostCount.text = _count.ToString();//"" + PlayerPrefs.GetInt("" + type);
			if (!check)
			{
				if (BoostCount() > 0)
					ShowPlus(false);
				else
					ShowPlus(true);
			}
		}
	}

//    void Update()
//    {
//        if (boostCount != null)
//        {
//            boostCount.text = "" + PlayerPrefs.GetInt("" + type);
//            if (!check)
//            {
//                if (BoostCount() > 0)
//                    ShowPlus(false);
//                else
//                    ShowPlus(true);
//            }
//        }
//    }
}
