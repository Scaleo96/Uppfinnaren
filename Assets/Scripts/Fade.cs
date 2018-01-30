using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    static public IEnumerator FadeIn(float time)
    {
        const float fadeAmount = 255;
        Image image;
        GameObject go = Instantiate(Resources.Load("FadeTransition", typeof(GameObject))) as GameObject;
        image = go.GetComponent<Image>();
        image.canvasRenderer.SetAlpha(0);
        Debug.Log("work");
        image.CrossFadeAlpha(fadeAmount, time, false);
        yield return new WaitForSeconds(time);
        image.CrossFadeAlpha(0, time, false);
        yield return new WaitForSeconds(time);
        Destroy(go);
    }
}
