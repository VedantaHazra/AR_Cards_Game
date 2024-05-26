using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

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

        claimText.text = "DOne";


    }
    public void InvokeOnLogIn()
    {
        OnLoggedIn.Invoke(Address);
        GetTokenBalance();
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

}



