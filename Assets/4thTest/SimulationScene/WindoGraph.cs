using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class WindoGraph : MonoBehaviour
{
    public Texture squareTexture;
    public Texture leftTriangleTexture;
    public Texture rightTriangleTexture;
    public GameObject newValInputBox;
    public List<int> valueList = new List<int>() { 90, 43, 54, 64, 12, 345, 56, 634, 23, 677, 43, 55, 67, 454, 34 };

    public void OnSubmit()
    {
        InputField inputField = newValInputBox.GetComponent<InputField>();
        valueList.Add(int.Parse(inputField.text));
    }

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;


    private void Awake()
    {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        ShowGraph(valueList);
    }

    private void CreateSquareAndTriangle(Vector2 squarePosition, Vector2 trianglePosition, float width, float squareHeight, float triangleHeight, string triangleDirection)
    {
        GameObject gameObject = new GameObject("square", typeof(RawImage));
        gameObject.transform.SetParent(graphContainer, false);
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rawImage.texture = squareTexture;
        rectTransform.anchoredPosition = squarePosition;
        rectTransform.sizeDelta = new Vector2(width, squareHeight);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);



        gameObject = new GameObject("triangle", typeof(RawImage));
        gameObject.transform.SetParent(graphContainer, false);
        rawImage = gameObject.GetComponent<RawImage>();
        rectTransform = gameObject.GetComponent<RectTransform>();
        //Debug.Log("triangle direction: " + triangleDirection);
        if (triangleDirection == "left") { rawImage.texture = leftTriangleTexture; }
        else { rawImage.texture = rightTriangleTexture; }
        rectTransform.anchoredPosition = trianglePosition;
        rectTransform.sizeDelta = new Vector2(width, triangleHeight);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3f, 3f);

        //keep everything on the lower left corner
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;

    }

    private void ShowGraph(List<int> valueList)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = FindHighestValueInAList(valueList);
        float xSize = graphWidth / (valueList.Count - 1);

        string triangleDirection = "";
        float squareHeight;
        float triangleHeight;
        float lastXPosition = 0f;
        float lastYPosition = 0f;
        GameObject lastCircleGameObject = null;
        if (valueList.Count - 1 != 0)
        {
            for (int i = 0; i < valueList.Count; i++)
            {
                float xPosition = (i * xSize);
                float yPosition = (valueList[i] / yMaximum) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                if (lastCircleGameObject != null)
                {
                    CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                    if (lastYPosition < yPosition) { squareHeight = lastYPosition; triangleDirection = "right"; triangleHeight = yPosition - lastYPosition; }
                    else { squareHeight = yPosition; triangleDirection = "left"; triangleHeight = lastYPosition - yPosition; }
                    CreateSquareAndTriangle(new Vector2(xPosition - (xSize / 2), squareHeight / 2), new Vector2(xPosition - (xSize / 2), squareHeight + (triangleHeight / 2)), xSize, squareHeight, triangleHeight, triangleDirection);
                }
                lastCircleGameObject = circleGameObject;
                lastXPosition = xPosition;
                lastYPosition = yPosition;
            }
        }
        else
        {
            float xPosition = (1 * graphWidth);
            float yPosition = (valueList[0] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));

            //append the single square
            CreateSquareAndTriangle(new Vector2(graphWidth / 2, graphHeight / 2), new Vector2(0, 0), graphWidth, graphHeight, 0, "left");
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    private static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public int FindHighestValueInAList(List<int> list)
    {
        if (list.Count == 0)
        {
            return 1;
        }
        int maxAge = int.MinValue;
        int itr = 0;
        foreach (int val in list)
        {
            itr = itr + 1;
            if (val > maxAge)
            {
                maxAge = val;
            }
        }
        return maxAge;
    }
}
