const express = require('express');
const Web3 = require('web3');
const app = express();

// Initialize Web3 with your Ethereum node provider
const web3 = new Web3('https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID');

// ABI and address of the smart contract
const contractABI = [...]; // Your smart contract ABI
const contractAddress = '0xYOUR_CONTRACT_ADDRESS'; // Address of your smart contract

// Create a contract instance
const contract = new web3.eth.Contract(contractABI, contractAddress);

// Endpoint to handle trade requests from Unity
app.post('/trade', async (req, res) => {
    const { toAddress, cardIdToTrade, cardIdWanted } = req.body;

    try {
        // Perform the trade
        const result = await contract.methods.trade(toAddress, cardIdToTrade, cardIdWanted).send({
            from: '0xYOUR_SENDER_ADDRESS', // Sender address
            gas: 3000000, // Adjust gas limit accordingly
        });

        // Return success response to Unity
        res.status(200).json({ success: true, transactionHash: result.transactionHash });
    } catch (error) {
        console.error('Trade failed:', error);
        // Return error response to Unity
        res.status(500).json({ success: false, error: error.message });
    }
});

// Start the server
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
