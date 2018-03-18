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

    [SerializeField]
    string[] randomFirstName;

    [SerializeField]
    string[] randomLastName;

    [SerializeField]
    string[] randomTitle;

    [SerializeField]
    string[] randomRole;
 
    [SerializeField]
    int extraCredits;

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

        for (int k = 0; k < extraCredits; k++)
        {
            GenerateCredits();
        }
    }

    private void GenerateCredits()
    {
        GameObject currentHeader = Instantiate(textPrefab, transform);
        int r = Random.Range(0, 100);

        if (r > 10)
        {
            currentHeader.GetComponent<Text>().text = randomTitle[Random.Range(0, randomTitle.Length)] + "   " + "-" + "   " + randomFirstName[Random.Range(0, randomFirstName.Length)] + " " + randomLastName[Random.Range(0, randomLastName.Length)];
        }
        else
        {
            currentHeader.GetComponent<Text>().text = randomRole[Random.Range(0, randomRole.Length)] + " " + randomTitle[Random.Range(0, randomTitle.Length)] + "   " + "-" + "   " + randomFirstName[Random.Range(0, randomFirstName.Length)] + " " + randomLastName[Random.Range(0, randomLastName.Length)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, creditsSpeed, 0);
        //transform.SetAsFirstSibling();

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

        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(Screen.width/2, Screen.width), Random.Range(0, Screen.height), Camera.main.farClipPlane / 2));

        GameObject go = Instantiate(imagePrefab, screenPosition, transform.rotation, FindObjectOfType<Canvas>().transform);

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
