// This loads the environment variables from the .env file
require('dotenv-extended').load();

// This startup file is meant to be used only when deploying any of the samples to an Azure Website

var botName = process.env.BOT || 'RealEstateBot';
var botModule = './' + botName + '/app'; 
require(botModule);