using UnityEngine;
using System.Collections;

public class ColorMix : MonoBehaviour {

    void OnEnable() {
        SoundBase.Instance.PlaySound(SoundBase.Instance.boostBomb);
    }

    public void OnFinish() {
        LevelManager.THIS.gameStatus = GameState.Playing;
        GameObject boom = Instantiate(Resources.Load("Prefabs/Effects/boom")) as GameObject;
        boom.transform.position = transform.position;
        Destroy(boom, 0.5f);
        Destroy(gameObject);
    }

    public void StartAction() {
        LevelManager.THIS.SetColorToRandomItems();
        SoundBase.Instance.PlaySound(SoundBase.Instance.boostColorReplace);


    }

    public void Explosion() {
        GameObject boom = Instantiate(Resources.Load("Prefabs/Effects/bomb_selection")) as GameObject;
        SoundBase.Instance.PlaySound(SoundBase.Instance.explosion);

        boom.transform.position = transform.position;
        Destroy(boom, 0.5f);
    }
}
