using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    GameObject headerPrefab;

    [SerializeField]
    GameObject logo;

    [SerializeField]
    float creditsSpeed = 5;

    [SerializeField]
    Sprite[] images;

    [SerializeField]
    GameObject imagePrefab;

    [SerializeField]
    string[] randomName;

    [SerializeField]
    string[] randomTitle;

    [SerializeField]
    string[] randomRole;
 
    [SerializeField]
    int extraCredits;

    [SerializeField]
    float creditsTime;
    float creditsTimer = 0;

    // DEPRECATED
    // VerticalLayoutGroup verticalLayoutGroup;

    float timer = 0;
    float duration = 5;

    // Use this for initialization
    void Start()
    {
        // DEPRECATED
        // verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

        transform.position = new Vector2(Screen.width / 4, -100);

        Instantiate(logo, transform);

        for (int i = 0; i < creditCatagory.Length; i++)
        {
            GameObject currentHeader = Instantiate(headerPrefab, transform);

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
            currentHeader.GetComponent<Text>().text = randomTitle[Random.Range(0, randomTitle.Length)] + "   " + "-" + "   " + randomName[Random.Range(0, randomName.Length)];
        }
        else
        {
            currentHeader.GetComponent<Text>().text = randomRole[Random.Range(0, randomRole.Length)] + " " + randomTitle[Random.Range(0, randomTitle.Length)] + "   " + "-" + "   " + randomName[Random.Range(0, randomName.Length)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, creditsSpeed, 0);
        transform.SetAsLastSibling();

        ImageTimer();
        Countdown();
        CancelCredits();
    }

    private void Countdown()
    {
        creditsTimer += Time.deltaTime;

        if (creditsTimer >= creditsTime)
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

    private void CancelCredits()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
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

        Vector3 screenPosition = new Vector3(Random.Range(Screen.width/2 + 200, Screen.width - 200), Random.Range(0 + 200, Screen.height - 200));

        Debug.Log(screenPosition);

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
