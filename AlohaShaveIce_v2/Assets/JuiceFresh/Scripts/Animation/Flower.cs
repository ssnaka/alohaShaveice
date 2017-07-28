using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flower : MonoBehaviour {
    Item item;
    // Use this for initialization
    void Start() {
        GetComponent<ParticleSystem>().Stop();
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(Vector3.back * Time.deltaTime * 1000);
        transform.position = new Vector3(transform.position.x, transform.position.y, -15f);
    }

    public void StartFly(Vector3 pos1, bool directFly = false) {
        GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(FlyCor(pos1, directFly));
    }

    IEnumerator FlyCor(Vector3 pos1, bool directFly = false) {
        Vector3 pos2 = Vector3.zero;
        yield return new WaitForFixedUpdate();

        transform.position = pos1;
        while (LevelManager.THIS.DragBlocked) {
            yield return new WaitForEndOfFrame();
        }
        List<Item> items = LevelManager.THIS.GetRandomItems(1);
        foreach (Item item1 in items) {
            item = item1;
            pos2 = item.transform.position;
        }
        if (item == null) {
            GetComponent<ParticleSystem>().Stop();
            GetComponent<SpriteRenderer>().enabled = false;
            yield break;
        }
        Item _item = item;
        _item.nextType = (ItemsTypes)Random.Range(1, 3);
        float startTime = Time.time;
        Vector3 startPos = pos1;
        float distance = Vector3.Distance(pos1, pos2);
        float speed = 5;
        if (directFly)
            speed *= 10;
        float fracJourney = 0;
        GetComponent<ParticleSystem>().gravityModifier = 0.1f;
        GetComponent<ParticleSystem>().Play();

        //        iTween.MoveTo(gameObject, iTween.Hash("position", pos2, "time", 1, "oncomplete", "AnimCallBack"));
        while (fracJourney < 1) {
            if (_item.awaken && _item.gameObject != null) {
                _item.nextType = ItemsTypes.NONE;
                StartFly(transform.position, directFly);
                yield break;
            }
            // speed += 0.2f;
            float distCovered = (Time.time - startTime) * speed;
            fracJourney = distCovered / distance;
			if (float.IsNaN (fracJourney))  //1.3
				fracJourney = 0;   //1.3
            transform.position = Vector3.Lerp(startPos, pos2, fracJourney);
            yield return new WaitForFixedUpdate();
        }

        GetComponent<ParticleSystem>().gravityModifier = 0;

        AnimCallBack();
    }

    void AnimCallBack() {
        GetComponent<ParticleSystem>().Stop();
        GetComponent<SpriteRenderer>().enabled = false;
        item.ChangeType();
        LevelManager.THIS.DragBlocked = false;

    }
}
