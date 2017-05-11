var Promise = require('bluebird');

const catalogItems = [
    {
        id: 'bc861179-46a5-4645-a249-7eba2a4d9846',
        name: 'Scott Gu - Favorite Shirt',
        description: 'Shiny red, ready to rock on Keynotes',
        price: 1.99,
        currency: 'USD',
        imageUrl: 'https://pbs.twimg.com/profile_images/565139568/redshirt_400x400.jpg'
    }
];

const catalog = {

    getItemById: (id) => Promise.resolve(
        catalogItems.find((e) => e.id.toLowerCase() === id.toLowerCase())),

    getPromotedItem: () => Promise.resolve(
        catalogItems[0])

};

module.exports = catalog;