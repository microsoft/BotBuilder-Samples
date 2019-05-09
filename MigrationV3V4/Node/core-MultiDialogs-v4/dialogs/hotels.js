const { AttachmentLayoutTypes, CardFactory } = require('botbuilder');
const store = require('../store');
const { 
    INITIAL_HOTEL_PROMPT, 
    CHECKIN_DATETIME_PROMPT, 
    HOW_MANY_NIGHTS_PROMPT 
} = require('../const');

const hotelsDialog = [
    async (stepContext) => {
        await stepContext.context.sendActivity('Welcome to the Hotels finder!');
        return await stepContext.prompt(
            INITIAL_HOTEL_PROMPT, {
                prompt: 'Please enter your destination'
            }
        );
    },
    async (stepContext) => {
        const destination = stepContext.result;
        stepContext.values.destination = destination;
        await stepContext.context.sendActivity(`Looking for hotels in ${destination}`);
        return stepContext.next();
    },
    async (stepContext) => {
        return await stepContext.prompt(
            CHECKIN_DATETIME_PROMPT, {
                prompt: 'When do you want to check in?'
            }
        );
    },
    async (stepContext) => {
        const checkinTime = stepContext.result[1].value;
        stepContext.values.checkinTime = checkinTime;
        return stepContext.next();
    },
    async (stepContext) => {
        return await stepContext.prompt(
            HOW_MANY_NIGHTS_PROMPT, {
                prompt: 'How many nights do you want to stay?'
            }
        );
    },
    async (stepContext) => {
        const numberofnights = stepContext.result;
        stepContext.values.numberofnights = parseInt(numberofnights);
        return stepContext.next();
    },
    async (stepContext) => {
        const destination = stepContext.values.destination;
        const checkIn = new Date(stepContext.values.checkinTime);
        const checkOut = addDays(checkIn, stepContext.values.numberofnights);

        await stepContext.context.sendActivity(`Ok. Searching for Hotels in ${destination} from ${checkIn.toDateString()} to ${checkOut.toDateString()}...`);
        const hotels = await store.searchHotels(destination, checkIn, checkOut);
        await stepContext.context.sendActivity(`I found in total ${hotels.length} hotels for your dates:`);
        
        const hotelHeroCards = hotels.map(createHotelHeroCard);

        await stepContext.context.sendActivity({
            attachments: hotelHeroCards,
            attachmentLayout: AttachmentLayoutTypes.Carousel
        });
        
        return await stepContext.endDialog();
    }
  
];

const addDays = (startDate, days) => {
    const date = startDate;
    date.setDate(date.getDate() + days);
    return date;
};

const createHotelHeroCard = (hotel) => {
    return CardFactory.heroCard(
        hotel.name,
        `${hotel.rating} stars. ${hotel.numberOfReviews} reviews. From ${hotel.priceStarting} per night.`,
        CardFactory.images([hotel.image]),
        CardFactory.actions([
            {
                type: 'openUrl',
                title: 'More details',
                value: `https://www.bing.com/search?q=hotels+in+${encodeURIComponent(hotel.location)}`
            }
        ])
    );
}

exports.hotelsDialog = hotelsDialog;