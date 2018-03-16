using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct CreditCatagory
{
    public string header;
    public string[] names;
}

[RequireComponent(typeof(Text))]
public class Credits : MonoBehaviour
{

    [SerializeField]
    CreditCatagory[] creditCatagory;

    [SerializeField]
    GameObject textPrefab;

    [SerializeField]
    float creditsSpeed = 5;

    [SerializeField]
    Sprite[] images;

    [SerializeField]
    GameObject imagePrefab;

    float timer = 0;
    float duration = 5;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < creditCatagory.Length; i++)
        {
            GameObject currentHeader = Instantiate(textPrefab, transform);

            currentHeader.GetComponent<Text>().fontStyle = FontStyle.Bold;
            currentHeader.GetComponent<Text>().fontSize *= 2;
            currentHeader.GetComponent<Text>().text = (creditCatagory[i].header);

            for (int j = 0; j < creditCatagory[i].names.Length; j++)
            {
                GameObject currentText = Instantiate(textPrefab, transform);
                currentText.GetComponent<Text>().text += (creditCatagory[i].names[j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, creditsSpeed, 0);
        transform.SetAsFirstSibling();

        ImageTimer();
    }

    private void ImageTimer()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            StartCoroutine(RandomImage());
            timer = 0;
        }
    }

    private IEnumerator RandomImage()
    {
        Image image;
        const float fadeAmount = 255;
        GameObject go = Instantiate(imagePrefab, (new Vector2(Random.Range(Screen.width/2 + 100, Screen.width), Random.Range(0, Screen.height))), transform.rotation, FindObjectOfType<Canvas>().transform);
        image = go.GetComponent<Image>();
        image.sprite = images[Random.Range(0, images.Length)];
        image.canvasRenderer.SetAlpha(0);
        image.CrossFadeAlpha(fadeAmount, 1, false);
        yield return new WaitForSeconds(duration);
        image.CrossFadeAlpha(0, 1, false);
        yield return new WaitForSeconds(1);
        Destroy(go);
    }
}
