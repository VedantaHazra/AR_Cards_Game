const express = require("express");
const Web3 = require("web3");
const contractABI = require("./contractABI.json"); // Load your contract ABI
const alchemyApiKey = "YOUR_ALCHEMY_API_KEY"; // Your Alchemy API key
const contractAddress = "0xYOUR_CONTRACT_ADDRESS"; // Your contract address
const senderAddress = "0xYOUR_SENDER_ADDRESS"; // The address initiating the trade
const toAddress = "0xRECIPIENT_ADDRESS"; // The address receiving the trade
const cardIdToTrade = 1; // The ID of the card to trade
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
