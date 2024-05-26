using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeCards : MonoBehaviour
{
    // Start is called before the first frame update
    public blockchainManager blockchainMana;
    void Start()
    {
        trade("0x1cD5a87BBc935739e69E3BcC726e442B104D8210", 3, 1);
    }
    public async void trade(string toAddress, int cardIdToTrade, int cardIdWanted)
    {
        await blockchainMana.TradeCards(toAddress, cardIdToTrade, cardIdWanted);

    }
}
