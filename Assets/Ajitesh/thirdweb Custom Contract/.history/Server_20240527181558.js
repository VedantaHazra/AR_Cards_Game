const express = require("express");
const Web3 = require("web3");
const contractABI = [
  {
    type: "event",
    name: "Trade",
    inputs: [
      {
        type: "address",
        name: "from",
        indexed: true,
        internalType: "address",
      },
      {
        type: "address",
        name: "to",
        indexed: true,
        internalType: "address",
      },
      {
        type: "uint256",
        name: "cardIdGave",
        indexed: false,
        internalType: "uint256",
      },
      {
        type: "uint256",
        name: "cardIdReceived",
        indexed: false,
        internalType: "uint256",
      },
    ],
    outputs: [],
    anonymous: false,
  },
  {
    type: "function",
    name: "assignCards",
    inputs: [
      {
        type: "address",
        name: "player",
        internalType: "address",
      },
      {
        type: "uint256",
        name: "card1",
        internalType: "uint256",
      },
      {
        type: "uint256",
        name: "card2",
        internalType: "uint256",
      },
    ],
    outputs: [],
    stateMutability: "nonpayable",
  },
  {
    type: "function",
    name: "cardOwnership",
    inputs: [
      {
        type: "address",
        name: "",
        internalType: "address",
      },
      {
        type: "uint256",
        name: "",
        internalType: "uint256",
      },
    ],
    outputs: [
      {
        type: "uint256",
        name: "",
        internalType: "uint256",
      },
    ],
    stateMutability: "view",
  },
  {
    type: "function",
    name: "trade",
    inputs: [
      {
        type: "address",
        name: "to",
        internalType: "address",
      },
      {
        type: "uint256",
        name: "cardIdToTrade",
        internalType: "uint256",
      },
      {
        type: "uint256",
        name: "cardIdWanted",
        internalType: "uint256",
      },
    ],
    outputs: [],
    stateMutability: "nonpayable",
  },
]; // Load your contract ABI
const alchemyApiKey = "snwRC_rqsG2d_LT-5BzTswfeYP2GPpuL"; // Your Alchemy API key
const contractAddress = "0xC5FD7C132C9Aa00be23756aC9046C89a4876c15a"; // Your contract address
const senderAddress = "0x1cD5a87BBc935739e69E3BcC726e442B104D8210"; // The address initiating the trade
const toAddress = "0xf904E2846F3aB662F8B4CC9bb6363B6017Ecb79f"; // The address receiving the trade
const cardIdToTrade = 3; // The ID of the card to trade
const cardIdWanted = 2; // The ID of the card wanted in return

const app = express();

// Initialize Web3 with Alchemy provider
const web3 = new Web3(
  new Web3.providers.HttpProvider(
    `https://eth-mainnet.alchemyapi.io/v2/${alchemyApiKey}`
  )
);

// Get contract instance
const contract = new web3.eth.Contract(contractABI, contractAddress);

// Define route to perform trade
app.get("/trade", async (req, res) => {
  try {
    // Perform the trade
    const transaction = contract.methods.trade(
      senderAddress,
      toAddress,
      cardIdToTrade,
      cardIdWanted
    );

    // Estimate gas for the transaction
    const gas = await transaction.estimateGas({ from: senderAddress });

    // Get the nonce
    const nonce = await web3.eth.getTransactionCount(senderAddress);

    // Build the transaction object
    const txObject = {
      nonce: nonce,
      gasLimit: web3.utils.toHex(gas),
      gasPrice: web3.utils.toHex(web3.utils.toWei("10", "gwei")), // Adjust gas price as needed
      to: contractAddress,
      data: transaction.encodeABI(),
    };

    // Sign the transaction
    const signedTx = await web3.eth.accounts.signTransaction(
      txObject,
      "YOUR_PRIVATE_KEY"
    );

    // Send the signed transaction
    const receipt = await web3.eth.sendSignedTransaction(
      signedTx.rawTransaction
    );

    console.log("Transaction hash:", receipt.transactionHash);
    res.status(200).send("Trade successful");
  } catch (error) {
    console.error("Trade failed:", error);
    res.status(500).send("Trade failed");
  }
});

// Start server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
