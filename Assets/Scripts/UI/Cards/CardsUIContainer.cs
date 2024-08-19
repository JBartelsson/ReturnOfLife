using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CardsUIContainer : MonoBehaviour
{
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform maxLeft;
    [SerializeField] private RectTransform maxRight;
    [SerializeField] private float maxCardWidth = 220f;
    private List<Vector3> cardTargetPositions = new List<Vector3>();
    private List<GameObject> debugList = new();
    
    private Vector3 ogPosition;
    private void Awake()
    {
        ogPosition = container.position;
    }

    public void CalculatePositions()
    {
        container.sizeDelta = new Vector2((GameManager.Instance.Deck.MaxHandSize - 1) * maxCardWidth, container.sizeDelta.y);
        container.position = ogPosition;
        Vector3 min = maxLeft.position;
        int maxCards = GameManager.Instance.Deck.HandCards.Count;
        Vector3 distanceVector = maxRight.position - maxLeft.position;
        cardTargetPositions.Clear();
        float width = (distanceVector * (float)(maxCards - 1) / (float)(GameManager.Instance.Deck.MaxHandSize - 1)).magnitude;
        float containerWidth = distanceVector.magnitude;
        float offset = (containerWidth - width) / 2;
        debugList.ForEach((x) => Destroy(x));
        for (int i = 0; i < maxCards; i++)
        {
            Vector3 targetPosition = min + distanceVector * ((float)i / (float)(GameManager.Instance.Deck.MaxHandSize - 1));
            cardTargetPositions.Add(targetPosition - new Vector3(offset, 0, 0));
            GameObject newObject = new GameObject($"lol{i}");
            newObject.AddComponent<RectTransform>();
            newObject.transform.SetParent(container);
            newObject.transform.position = targetPosition;
            debugList.Add(newObject);
        }

        
        // container.position = ogPosition + new Vector3(offset, 0, 0);
    }

    public Vector3 GetCardTargetPositionByIndex(int index)
    {
        if (index >= cardTargetPositions.Count || index < 0) return Vector3.zero;
        
        return cardTargetPositions[index];
    }
}
