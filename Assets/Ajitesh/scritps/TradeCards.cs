// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TradeCards : MonoBehaviour
// {
//     public blockchainManager blockchainMana;

//     void Start()
//     {
//         // Example usage of the trade method, can be uncommented if needed
//         // trade("0x1cD5a87BBc935739e69E3BcC726e442B104D8210", 3, 1);
//     }

//     public void Trade(string toAddress, int cardIdToTrade, int cardIdWanted)
//     {
//         if (blockchainMana == null)
//         {
//             Debug.LogError("blockchainManager is not assigned.");
//             return;
//         }

//         Debug.Log($"Initiating trade: To Address = {toAddress}, Card ID to Trade = {cardIdToTrade}, Card ID Wanted = {cardIdWanted}");
//         blockchainMana.TradeCards(toAddress, cardIdToTrade, cardIdWanted);
//         Debug.Log("Trade being completed......");
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TradeCards : MonoBehaviour
{

    public void khuchNhi()
    {

    }

    public void Trade(string toAddress)
    {
        StartCoroutine(CallTradeAPI(toAddress, "2", "4"));
    }

    private IEnumerator CallTradeAPI(string toAddress, string cardIdToTrade, string cardIdWanted)
    {
        string serverUrl = $"http://localhost:3000/trade/{toAddress}/{cardIdToTrade}/{cardIdWanted}";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(serverUrl, ""))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Trade request failed: " + request.error);
            }
            else
            {
                Debug.Log("Trade request sent successfully.");
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }

}
