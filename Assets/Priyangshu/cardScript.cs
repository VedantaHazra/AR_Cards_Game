using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// textmeshpro


public class cardScript : MonoBehaviour
{

    public Card card;

    public bool canUse = true;

    public Button button;
    public TMP_Text levelText;
    public TMP_Text damageText;
    public TMP_Text cooldownText;

    public GameObject cardImage;

    // Start is called before the first frame update
    void Start()
    {
        // cardImage = transform.GetChild(0).gameObject;
        cardImage.GetComponent<UnityEngine.UI.Image>().sprite = card.artwork;
        levelText.text = card.level.ToString();
        damageText.text = card.damage.ToString();
        cooldownText.text = card.cooldownTime.ToString();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void useCard(){
        Debug.Log("Card Used:");
        Debug.Log(card.name);
        Debug.Log(card.type);
        Debug.Log(card.damage);
        Debug.Log(card.cooldownTime);
        card.useCard();

        cooldown();
    }

    public void upgradeCard(){
        card.upgradeCard();
        levelText.text = card.level.ToString();
        damageText.text = card.damage.ToString();
        cooldownText.text = card.cooldownTime.ToString();
    }

    //cooldown timer
    public void cooldown(){
        StartCoroutine(cooldownTimer());
        // GetComponent<TMPro.ui>().interactable = false;
        button.GetComponent<Button>().interactable = false;
        Debug.Log("Cooldown Started");
    }
    IEnumerator cooldownTimer(){
        canUse = false;
        yield return new WaitForSeconds(card.cooldownTime);
        canUse = true;
        button.GetComponent<Button>().interactable = true;
        Debug.Log("Cooldown Ended");
        // GetComponent<UnityEngine.UI.Button>().interactable = true;
    }
}
