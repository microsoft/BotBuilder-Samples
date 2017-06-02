# Using Search to Create Data Driven Bots

In this demo I'll demonstrate how to use Azure Document DB, Azure Search and the Microsoft Bot Framework to build a bot that searches and filters over an underlying dataset.

[![Deploy to Azure][Deploy Button]][Deploy Node/KnowledgeBot]

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy Node/KnowledgeBot]: https://azuredeploy.net

## Background
More and more frequently we're seeing the value in bots that can reason over underlying data. These bots can help provide users with information about events, products, telemetry etc. Where it's certainly possible to connect a bot directly to a database and perform queries against it, we've found that using a search engine over our data is particularly helpful for two big things: 

1. Indexing and searching an underlying dataset to return the results that best match user input. 

    * For one, fuzzy search keeps users from having to type exact matches (e.g. "who is jennifer?" instead of "jennifer marsman", "impala" instead of "Tame Impala")

        <img src="./images/fuzzySearch.PNG" alt="Screenshot" style="width: 300px; padding-left: 40px;"/>
        <img src="./images/fuzzySearch2.PNG" alt="Screenshot" style="width: 530px;"/>

    * Search scores allow us to determine the confidence that we have about a specific search - allowing us to decide whether a piece of data is what a user is looking, order results based on our confidence, and curb our bot output based on confidence (e.g. "Hmm... were you looking for any of these events?" vs "Here is the event that best matches your search:") 

    <img src="./images/searchScore1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>
    <img src="./images/searchScore2.PNG" alt="Screenshot" style="width: 536px;"/>

2. Guiding a user through a conversation that facets and filters a dataset until it finds what a user is looking for. In this example, we use search to determine all of the facets of the underlying database's fields to quickly guide the conversation:

    <img src="./images/guidedConvo1.png" alt="Screenshot" style="width: 300px; padding-left: 40px;"/>
    <img src="./images/guidedConvo2.png" alt="Screenshot" style="width: 300px;"/>
    <img src="./images/guidedConvo3.png" alt="Screenshot" style="width: 300px;"/>
    <img src="./images/guidedConvo4.png" alt="Screenshot" style="width: 300px;"/>


I'm going to demonstrate the creation of a simple bot that searches and filters over a dataset of classical musicians. First we'll set up our database, then we'll create our search service, and then we'll build our bot.

## Database Setup - DocumentDB 
I'll start by noting the musicianData JSON file found in the data folder of this project. Each JSON object is made up of four properties: musician name, era, description, and image url. Our goal will be to allow users to quickly find a specific musician or filter musicians by their different eras. Our dataset only contains 19 musicians, but this approach can easily scale to millions of datapoints. Azure Search is capable of indexing data from several data sources including Document DB, Blob Storage, Table Storage and Azure SQL. We'll use Document DB as a demonstration. 

### Create a Document DB database and collection. 
1. Navigate to Document DB in the Azure Portal 

    <img src="./images/docDB1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>
                                  
2. Create Doc DB account

    <img src="./images/docDB2.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

3. Create collection/add new DB

    <img src="./images/docDB3.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>


### Upload JSON data
Now that we've got our database and collection set up, let's go ahead and push our JSON data up. We can do this programatically, but for the
sake of simplicity I'm going to use the Document DB Data Migration Tool (documented here https://azure.microsoft.com/en-us/documentation/articles/documentdb-import-data/).

1. Once you've got the tool, navigate to the musician JSON data: 

    <img src="./images/dtui1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

2. Fill in target information

    1. Get connection strings from portal

        <img src="./images/dtui2.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

    2. Be sure to add Database = <DatabaseName>; to your connection string

        <img src="./images/dtui3.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

    3. Then upload your data: 

        <img src="./images/dtui4.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

    To see that our data has uploaded, we can go back to the portal, click query explorer and run the default query `SELECT * FROM c`:
        <img src="./images/queryexplorer1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

3. Create your Azure Search index

    1. Create an Azure Search service

        <img src="./images/search1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

    2. Import Data from your Document DB collection

        <img src="./images/search2.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>
    
    3. Create your Azure Search index

        Here's where the magic starts to happen. You can see that Azure Search has accessed our data and pulled in each parameter of the JSON objects. Now we get to decide which of these parameters we want to search over, facet over, filter by and retrieve. Again we could generate our indeces programically, and in more complex use cases we would, but for the sake of simplicity we'll stick to the portal UI. Given that we want access to all of these properties we'll go ahead and make them all retrievable. We want to be able to facet (more details about faceting to come) and filter over musician's eras. Finally, we'll mark name as searchable so that our bot can search for musicians by their names. 

        <img src="./images/search3.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

4. Create your Azure Search indexer
    As our data is subject to change, we need to be able to reindex that data. Azure Search allows you to index on a schedule or on demand, but for this demo we'll index once only.

    <img src="./images/search4.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

5. Use the Search explorer

    We can verify that our index is properly functioning by using the Azure Search Explorer to enter example searches, filters and facets. This can be a very useful tool in testing out queries as you develop your bot. Note: If you enter a blank query the explorer should return all of your data. 
    
    Let's try three different queries:
    
    * `"Frederic"`

    Given that our index searches over musician name, a search of "Frederic" returns the information for "Frederic Chopin" along with a search score. The search score represents the confidence that Azure Search has regarding each result. 

    <img src="./images/search5.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>
    
    If we search instead for "Johannes", we will get two pertinent results: one for Johannes Sebastian Bach and the other for Johannes Brahms

    * `facet=Era`

    Faceting allows us to see the different examples of a parameter and their corresponding counts. You can see here that the JSON response from the search API tells us that there are 11 Romantic musicians, 3 Classical musicians, 2 Baroque musicians and 1 Modernist musician:
            
    <img src="./images/search7.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

    This information will allow us to guide the conversation our bot can have. If a user wishes to see musicians by era, our bot can quickly and efficiently find all the eras that are possible and present them as options to the user. 

    * `$filter=Era eq 'Romantic'`

    <img src="./images/search6.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

4. Build your Bot

    The bot I will demonstrate is built in Node.js and C#. For understanding C# bot more, please see [this document](./CSharp/readme.md).
    This bot will be fairly simple, but if you're new to bot building several of the concepts might be foreign. For a quick ramp up check out [aka.ms/botcourse](http://aka.ms/botcourse), specifically the sections about setting up a node project, using cards and using dialogs. 

    All of our connector logic is being stored in the connectorSetup.js. Here's where you would enter your appId and appPassword if you were going to 
    make this bot live or connect it to a non-emulator channel. 

    Let's dive into the dialog logic now:

    All messages get routed into the root ('/') dialog. From here, we replace the dialog with the promptButtons dialog. 
    ```javascript
    bot.dialog('/', [
        function (session) {
            session.replaceDialog('/promptButtons');
        }
    ]);
    ```
    In the promptButtons dialog we use the Prompts.choice method to prompt the user with our choices (in this case Musician Explorer and Musician Search) defined in the `choices` array. Once the user answers, we move into the next function which uses a switch statement to decide which dialog to route us to. Note that the musicianExplorer and musicianSearch dialogs each have their own .js file in the 'dialogs' folder and were included at the start of [app.js](./app.js).

    ```javascript
    bot.dialog('/promptButtons', [
        function (session) {
            var choices = ["Musician Explorer", "Musician Search"]
            builder.Prompts.choice(session, "How would you like to explore the classical music bot?", choices);
        },
        function (session, results) {
            if (results.response) {
                var selection = results.response.entity;
                // route to corresponding dialogs
                switch (selection) {
                    case "Musician Explorer":
                        session.replaceDialog('/musicianExplorer');
                        break;
                    case "Musician Search":
                        session.replaceDialog('/musicianSearch');
                        break;
                    default:
                        session.reset('/');
                        break;
                }
            }
        }
    ]);
    ```
    The musician search dialog first prompts the user to type in the name of the musician that he/she is looking for:

    ```javascript
    bot.dialog('/musicianSearch', [
        function (session) {
            builder.Prompts.text(session, "Type in the name of the musician you are searching for:");
        },
    ```
    It then gets the name the user typed in and passes in 'search= ' + name to the searchQueryStringBuilder to generate a basic search against our index. If it gets results from the query it routes us to showResults, which we can think of as a view layer. Note that we pass {results} to the showResults dialog. The showResults dialog receives these as a property as part of an `args` parameter which we will explore later. If the search renders no results then we send `session.endDialog("No musicians by the name \'" + name + "\' found");` to the user. 

    ```javascript
    function (session, results) {
        //Sets name equal to resulting input
        var name = results.response;
        var queryString = searchQueryStringBuilder('search= ' + name);
        performSearchQuery(queryString, function (err, result) {
            if (err) {
                console.log("Error when searching for musician: " + err);
            } else if (result && result['value'] && result['value'][0]) {
                //If we have results send them to the showResults dialog (acts like a decoupled view)
                session.replaceDialog('/showResults', { result });
            } else {
                session.endDialog("No musicians by the name \'" + name + "\' found");
            }
        })
    }
    ```
    Note that our error handling for this example simply logs the error to console - in a real world bot we would want to be more involved in 
    our error handling. 

    Our musician explorer is a bit more involved. First it gathers our era facets and prompts the user to choose which one he/she is interested in. 
    Again we create a queryString (this time passing 'facet=Era') and perform our search query, which gives us a JSON response of all of the eras of musicians that are represented in our index:

    ```javascript
        bot.dialog('/musicianExplorer', [
            function (session) {
                //Syntax for faceting results by 'Era'
                var queryString = searchQueryStringBuilder('facet=Era');
                performSearchQuery(queryString, function (err, result) {
                    if (err) {
                        console.log("Error when faceting by era:" + err);
                    } else if (result && result['@search.facets'] && result['@search.facets'].Era) {
                        eras = result['@search.facets'].Era;
                        var eraNames = [];
                        //Pushes the name of each era into an array
                        eras.forEach(function (era, i) {
                            eraNames.push(era['value'] + " (" + era.count + ")");
                        })    
                        //Prompts the user to select the era he/she is interested in
                        builder.Prompts.choice(session, "Which era of music are you interested in?", eraNames);
                    } else {
                        session.endDialog("I couldn't find any genres to show you");
                    }
                })
    ```

    Once the user selects the era that they are interested in we perform a filter query, passing $filter=Era eq selectedEra

    ```javascript
    function (session, results) {
        //Chooses just the era name - parsing out the count
        var era = results.response.entity.split(' ')[0];;
        //Syntax for filtering results by 'era'. Note the $ in front of filter (OData syntax)
        var queryString = searchQueryStringBuilder('$filter=Era eq ' + '\'' + era + '\'');
        performSearchQuery(queryString, function (err, result) {
            if (err) {
                console.log("Error when filtering by genre: " + err);
            } else if (result && result['value'] && result['value'][0]) {
                //If we have results send them to the showResults dialog (acts like a decoupled view)
                session.replaceDialog('/showResults', { result });
            } else {
                session.endDialog("I couldn't find any musicians in that era :0");
            }
        })
    }
    ```
    This gives us all of the musicians that map to the era the user chose. Once we have results, we again send them to our showResults dialog.

    The `performSearchQuery` function performs a basic query using the request library. Note, we're performing a GET for this example, but for production bots/apps you may consider using a POST so that you can place you api key in the POST header

    ```javascript
        global.performSearchQuery = function (queryString, callback) {
            request(queryString, function (error, response, body) {
                if (!error && response && response.statusCode == 200) {
                    var result = JSON.parse(body);
                    callback(null, result);
                } else {
                    callback(error, null);
                }
            })
        }
    ```

    The showResults dialog receives the results from the musicianExplorer and musicianSearch dialogs as properties of the `args` parameter. It then creates a new message with a carousel layout, `var msg = new builder.Message(session).attachmentLayout(builder.AttachmentLayout.carousel);`, and adds a hero card attachment with the name, era, search score, description and image for each musician.

    ```javascript
            function (session, args) {
                var msg = new builder.Message(session).attachmentLayout(builder.AttachmentLayout.carousel);
                if (args.result) {
                    args.result['value'].forEach(function (musician, i) {
                        var img = musician.imageURL;
                        msg.addAttachment(
                            new builder.HeroCard(session)
                                .title(musician.Name)
                                .subtitle("Era: " + musician.Era + " | " + "Search Score: " + musician['@search.score'])
                                .text(musician.Description)
                                .images([builder.CardImage.create(session, img)])
                        );
                    })
                    session.endDialog(msg);
                }
            }
        ])
    ```

    Finally, let's test our bot out. Either deploy your bot to an Azure web app and fill in the process.env variables in the portal, or add your search credentials in the config.js file. I will demonstrate the bot working in the bot framework emulator, but if deployed, this bot could be enabled on several different channels. 

    Musician Explorer functionality: 

    <img src="./images/musicianExplorer1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>
    <br>
    <img src="./images/musicianExplorer2.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>

    Note that the search scores returned with the filtered results are always 1. This is because a filter is essentially an exact match

    Musician Search functionality

    <img src="./images/musicianSearch1.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>
    <br>
    <img src="./images/musicianSearch2.PNG" alt="Screenshot" style="width: 500px; padding-left: 40px;"/>



