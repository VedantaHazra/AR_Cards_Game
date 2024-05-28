const express = require("express");
const Web3 = require("web3");
const app = express();

// Initialize Web3 with your Ethereum node provider
const web3 = new Web3("https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID");

// ABI and address of the smart contract
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
]; // Your smart contract ABI
const contractAddress = "0xYOUR_CONTRACT_ADDRESS"; // Address of your smart contract

// Create a contract instance
const contract = new web3.eth.Contract(contractABI, contractAddress);

// Endpoint to handle trade requests from Unity
app.post("/trade", async (req, res) => {
  const { toAddress, cardIdToTrade, cardIdWanted } = req.body;

  try {
    // Perform the trade
    const result = await contract.methods
      .trade(toAddress, cardIdToTrade, cardIdWanted)
      .send({
        from: "0xYOUR_SENDER_ADDRESS", // Sender address
        gas: 3000000, // Adjust gas limit accordingly
      });

    // Return success response to Unity
    res
      .status(200)
      .json({ success: true, transactionHash: result.transactionHash });
  } catch (error) {
    console.error("Trade failed:", error);
    // Return error response to Unity
    res.status(500).json({ success: false, error: error.message });
  }
});

// Start the server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
