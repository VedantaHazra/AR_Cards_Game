using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Numerics;

public class blockchainManager : MonoBehaviour
{
    public UnityEvent<string> OnLoggedIn;

    public string Address { get; private set; }

    public static blockchainManager Instance { get; private set; }

    public Button claimCurrency;
    public TextMeshProUGUI claimText;
    public TextMeshProUGUI Balance;

    //public script
    public string score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public async void Login(string authProvider)
    {
        var sdk = ThirdwebManager.Instance.SDK;
        AuthProvider provider = AuthProvider.Google;
        switch (authProvider)
        {
            case "google":
                provider = AuthProvider.Google;
                break;
            case "apple":
                provider = AuthProvider.Apple;
                break;
            case "facebook":
                provider = AuthProvider.Facebook;
                break;
        }

        var connection = new WalletConnection(
            provider: WalletProvider.SmartWallet,
            chainId: 11155111,
            personalWallet: WalletProvider.LocalWallet,
            authOptions: new AuthOptions(authProvider: provider)
        );

        Address = await sdk.Wallet.Connect(connection);//yeh Wallet me W capital hai docs me galat likha hai
        Debug.Log("yeh wallet address hai ");
        Debug.Log(Address);
        InvokeOnLogIn();

    }


    internal async Task SubmitScore(float distanceTravelled)
    {
        Debug.Log($"Submitting score of {distanceTravelled} to blockchain for address {Address}");
        var contract = ThirdwebManager.Instance.SDK.GetContract(
            "0x9d9a1f4c1a685857a5666db45588aa3d5643af9f",
            "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"ScoreAdded\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getRank\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"rank\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"submitScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]"
        );
        await contract.Write("submitScore", (int)distanceTravelled);
    }

    internal async Task<int> GetRank()
    {
        var contract = ThirdwebManager.Instance.SDK.GetContract(
            "0x9d9a1f4c1a685857a5666db45588aa3d5643af9f",
            "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"ScoreAdded\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getRank\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"rank\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"score\",\"type\":\"uint256\"}],\"name\":\"submitScore\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]"
        );
        var rank = await contract.Read<int>("getRank", Address);
        Debug.Log($"Rank for address {Address} is {rank}");
        return rank;
    }

    public async void ClaimScore()
    {
        claimText.text = "Clamming...";
        claimCurrency.interactable = false;
        var sdk = ThirdwebManager.Instance.SDK;
        var contract = sdk.GetContract("0x5881C10a844dbD2d8B5A037c23354286F65C8193");
        var result = await contract.ERC20.ClaimTo(Address, score);
        claimText.text = "Done";



    }
    public void InvokeOnLogIn()
    {
        OnLoggedIn.Invoke(Address);
        GetTokenBalance();
        TradeCards("0x1cD5a87BBc935739e69E3BcC726e442B104D8210", 3, 1);
    }

    public async void GetTokenBalance()
    {
        Debug.Log("Get Token balanace");
        Debug.Log(Address);
        var sdk = ThirdwebManager.Instance.SDK;
        var contract = sdk.GetContract("0x5881C10a844dbD2d8B5A037c23354286F65C8193");
        var balance = await contract.ERC20.BalanceOf(Address);
        Balance.text = "balance : " + balance.displayValue;

    }


    // Function to trade cards between two users------------------------------------------------------------------------------------------

    public void Tradefunc(string toAddress, int cardIdToTrade, int cardIdWanted)
    {
        TradeCards(toAddress, cardIdToTrade, cardIdWanted);
    }

    // Function to get the user's wallet address
    public async Task<string> GetUserWalletAddress()
    {
        // var sdk = ThirdwebManager.Instance.SDK;
        // try
        // {
        //     var wallet = sdk.Wallet;
        //     var address = await wallet.GetAddress();
        //     Debug.Log("User Wallet Address: " + address);
        //     return address;
        // }
        // catch (System.Exception e)
        // {
        //     Debug.Log("Failed to get wallet address: " + e.Message);
        return null;
        // }
    }

    // Function to show the cards a user has
    public async void ShowUserCards(string userAddress)
    {
        Debug.Log("Show card of user : " + userAddress);
        var sdk = ThirdwebManager.Instance.SDK;
        string json = "[{\"type\":\"event\",\"name\":\"Trade\",\"inputs\":[{\"type\":\"address\",\"name\":\"from\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"address\",\"name\":\"to\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"cardIdGave\",\"indexed\":false,\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"cardIdReceived\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"assignCards\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"card1\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"card2\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"cardOwnership\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"trade\",\"inputs\":[{\"type\":\"address\",\"name\":\"to\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"cardIdToTrade\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"cardIdWanted\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]";
        var contract = sdk.GetContract("0xC5FD7C132C9Aa00be23756aC9046C89a4876c15a", json);

        try
        {
            var card1 = await contract.Read<BigInteger>("cardOwnership", userAddress, new BigInteger(0));
            var card2 = await contract.Read<BigInteger>("cardOwnership", userAddress, new BigInteger(1));
            Debug.Log("User " + userAddress + " owns cards: " + card1 + ", " + card2);
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to retrieve user cards: " + e.Message);
        }
    }

    // Function to assign cards to a user
    public async void AssignCards(string userAddress, BigInteger card1, BigInteger card2)
    {
        var sdk = ThirdwebManager.Instance.SDK;
        string json = "[{\"type\":\"event\",\"name\":\"Trade\",\"inputs\":[{\"type\":\"address\",\"name\":\"from\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"address\",\"name\":\"to\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"cardIdGave\",\"indexed\":false,\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"cardIdReceived\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"assignCards\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"card1\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"card2\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"cardOwnership\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"trade\",\"inputs\":[{\"type\":\"address\",\"name\":\"to\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"cardIdToTrade\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"cardIdWanted\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]";
        var contract = sdk.GetContract("0xC5FD7C132C9Aa00be23756aC9046C89a4876c15a", json);
        try
        {
            var result = await contract.Write("assignCards", userAddress, card1, card2);
            Debug.Log("Assigned cards " + card1 + " and " + card2 + " to user " + userAddress);
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to assign cards: " + e.Message);
        }
    }

    public async Task TradeCards(string toAddress, int cardIdToTrade, int cardIdWanted)
    {
        Debug.Log("Trade Cards");
        var sdk = ThirdwebManager.Instance.SDK;
        string json = "[{\"type\":\"event\",\"name\":\"Trade\",\"inputs\":[{\"type\":\"address\",\"name\":\"from\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"address\",\"name\":\"to\",\"indexed\":true,\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"cardIdGave\",\"indexed\":false,\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"cardIdReceived\",\"indexed\":false,\"internalType\":\"uint256\"}],\"outputs\":[],\"anonymous\":false},{\"type\":\"function\",\"name\":\"assignCards\",\"inputs\":[{\"type\":\"address\",\"name\":\"player\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"card1\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"card2\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"},{\"type\":\"function\",\"name\":\"cardOwnership\",\"inputs\":[{\"type\":\"address\",\"name\":\"\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"outputs\":[{\"type\":\"uint256\",\"name\":\"\",\"internalType\":\"uint256\"}],\"stateMutability\":\"view\"},{\"type\":\"function\",\"name\":\"trade\",\"inputs\":[{\"type\":\"address\",\"name\":\"to\",\"internalType\":\"address\"},{\"type\":\"uint256\",\"name\":\"cardIdToTrade\",\"internalType\":\"uint256\"},{\"type\":\"uint256\",\"name\":\"cardIdWanted\",\"internalType\":\"uint256\"}],\"outputs\":[],\"stateMutability\":\"nonpayable\"}]";
        var contract = sdk.GetContract("0xC5FD7C132C9Aa00be23756aC9046C89a4876c15a", json);

        Debug.Log("Contract retrieved in TradeCards");
        try
        {
            var balance = await contract.ERC20.BalanceOf(Address);
            BigInteger balanceValue = BigInteger.Parse(balance.value);
            BigInteger requiredBalance = new BigInteger(1000); // Set the required balance as per your requirements

            if (balanceValue >= requiredBalance)
            {
                var result = await contract.Write("trade", toAddress, cardIdToTrade, cardIdWanted);
                Debug.Log("Trade successful: " + result);
            }
            else
            {
                Debug.Log("Insufficient balance for trade");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Trade failed: " + e.Message);
        }
    }




}





