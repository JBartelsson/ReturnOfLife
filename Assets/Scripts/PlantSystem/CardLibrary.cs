using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CardLibrary : MonoBehaviour
{
    [FormerlySerializedAs("cardData")] [FormerlySerializedAs("cardInstances")] [SerializeField] private List<CardData> cardDataList;

    public class CardReference
    {
        public CardData.LifeformTypeEnum lifeformType;
        public CardData CardData;
        
    }
    private void Awake()
    {
        Debug.Log($"Instance on Awake is {Instance}");
        if (Instance == null)
        {
            Instance = this;
            //Resetting Parenting structure so dontdestroyonload does work
            this.transform.parent = null;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Game Manager already exists!");
        }
    }

    public static CardLibrary Instance { get; private set; }

    public CardData GetCardDataOfLifeFormType(CardData.LifeformTypeEnum lifeformType)
    {
        return cardDataList.FirstOrDefault((x) => x.LifeformType == lifeformType);
    }
    public CardCollection ReturnAmountOfCardInstances(int amount)
    {
        if (amount > cardDataList.Count) return null;
        CardCollection tempCardCollection = new CardCollection(cardDataList);
        CardCollection returnCardCollection = new CardCollection();
        tempCardCollection.ShuffleCardCollection();
        for (int i = 0; i < amount; i++)
        {
            returnCardCollection.AddCardToCollection(tempCardCollection.CardsInCollection[i]);
        }

        return returnCardCollection;
    }
}
