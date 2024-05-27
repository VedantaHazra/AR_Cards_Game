using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandler : MonoBehaviour
{

    public GameObject cardPrefab;
    public List<GameObject> cards;

    float width,height;

    public GameObject player;

    public Card[] cardType;


    // Start is called before the first frame update
    void Start()
    {
        width = GetComponent<RectTransform>().rect.width;
        height = GetComponent<RectTransform>().rect.height;
        player = GameObject.Find("Player");

        addRandomCard();
        addRandomCard();
        addRandomCard();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addRandomCard()
    {
        //instantiate new card as child of this object
        GameObject newCard = Instantiate(cardPrefab, this.transform);
        //add card to list
        cards.Add(newCard);
        newCard.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        newCard.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        newCard.GetComponent<RectTransform>().localPosition = new Vector3( -150f - ((cards.Count-1)*300) + width/2, -50f + height/2, 0f);

        //set card type
        newCard.GetComponent<cardScript>().card = cardType[Random.Range(0, cardType.Length)];
    }

    public void removeCard(GameObject card)
    {
        cards.Remove(card);
        Destroy(card);
        for(int i = 0; i < cards.Count; i++)
        {
            cards[i].GetComponent<RectTransform>().localPosition = new Vector3(-150f - (i * 300) + width / 2, -50f + height / 2, 0f);
        }
    }

    public void addSpecificCard(Card card)
    {
        //instantiate new card as child of this object
        GameObject newCard = Instantiate(cardPrefab, this.transform);
        //add card to list
        cards.Add(newCard);
        newCard.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        newCard.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        newCard.GetComponent<RectTransform>().localPosition = new Vector3(-150f - ((cards.Count - 1) * 300) + width / 2, -50f + height / 2, 0f);

        //set card type
        newCard.GetComponent<cardScript>().card = card;
    }
}
