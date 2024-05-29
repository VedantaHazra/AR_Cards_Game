using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{

    public GameObject cardPrefab;
    public List<GameObject> sell_cards_gos;
    public List<GameObject> buy_cards_gos;
    public List<Card> cards_toSell;

    public List<Card> cards_toBuy;

    public float width,height;


    public TMP_InputField addressInput;
    public string address;

    // public GameObject player;

    public Card[] cardSriptableObjects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getCards(){
        getSellCards();
        getBuyCards();
    }


    void getSellCards(){

        address = addressInput.text;

        for(int i=0;i<cardSriptableObjects.Length;i++){
            if(cardSriptableObjects[i].isPossessed){
                cards_toSell.Add(cardSriptableObjects[i]);
                addCardGO(cardSriptableObjects[i],"sell");
            }
        }
    }

    void getBuyCards(){

    }

    void addCardGO(Card card, String type){
        GameObject newCard = Instantiate(cardPrefab, this.transform);
        newCard.GetComponent<cardScript>().card = card;
        newCard.GetComponent<cardScript>().uiManager = this;
        // newCard.transform.localScale = new Vector3(.45f,.45f,.45f);
        
        if(type=="sell"){
            sell_cards_gos.Add(newCard);
            newCard.GetComponent<RectTransform>().localPosition = new Vector3( -350f + ((sell_cards_gos.Count-1)*140), -88f, 0f);
            
        }
        if(type=="buy"){
            buy_cards_gos.Add(newCard);
            newCard.GetComponent<RectTransform>().localPosition = new Vector3( 80f + ((buy_cards_gos.Count-1)*140), -88f, 0f);
            
        }
    }

    public void sellCard(Card card){
        card.isPossessed = false;
        cards_toSell.Remove(card);
        // player.GetComponent<Player>().money += card.cost;
        // player.GetComponent<Player>().updateMoney();
        // player.GetComponent<Player>().updateCards();
    }
}
