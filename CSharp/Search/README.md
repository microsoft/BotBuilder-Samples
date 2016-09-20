# Search Dialogs Sample

Depending on the nature any given bot, sometimes you want to present structured dialogs that take users through tight paths, while in other cases you want to 
help users navigate a large amount of content. In many cases you'll have a mix of dialogs of both kinds in a single bot.  

These samples illustrate how to approach dialogs that need to help the user navigate large amounts of content, creating a data-driven exploration experience. 
We model content as a catalog of items where each item has several attributes we may want to use for navigation, keyword search or display.

We use [Azure Search](https://azure.microsoft.com/en-us/services/search/) as the backend for these dialogs. This is an Azure service that offers most of the 
pieces of functionality we needed, including keyword search, built-in linguistics, custom scoring, faceted navigation and more. Azure Search can also index
content from various sources (Azure SQL DB, DocumentDB, Blob Storage, Table Storage), supports "push" indexing for other sources of data, and can crack open
PDFs, Office documents and other formats containing unstructured data. The content catalog goes into an Azure Search index, which we can then query from 
dialogs.

We included a few different dialogs that are ready to use directly, or can be subtyped to override various pieces of functionality as needed:
* SearchSelectRefinerDialog helps users pick a refiner (facet). It's a simple wrapper around a "choice" prompt dialog that can use a shared instance of
SearchQueryBuilder to ensure you don't prompt users for a field you already refined on.
* SearchRefineDialog allows users to see different values for a given field and select one. This is typically used for filtering later on but can be applied
to any case where you want to list distinct values for a given field in the catalog and let the user pick one.
* SearchDialog offers a complete keyword search + refine experience over a search index, and uses the other search dialogs as building blocks. Users can explore 
the catalog by refining (using facets) and by using keyword search. They can also select items and review their selection. At the end of this dialog a list of 
one or more selected items is returned. You'll need to subtype this class and at a minimum override GetTopRefiners() (to list refiners (facets) to expore) and 
ToSearchHit() (to convert your index entries into a common representation that can be rendered).

To stitch together multiple instances of these dialogs and have filters and other search options carry over, you can use a shared instance of 
SearchQueryBuilder, which captures all the search-related state.

We included two samples here:
1. RealEstateBot is a bot for exploring a real estate catalog. It starts by taking an arbitrary set of keywords. From there you can go back and forth between 
keyword search and refinement using region, city and type of property. You can pick one or more properties and at the end you'll get a list of your choices
(a real bot would probably contact your agent with your elections, or send you a summary email for future reference).
2. JobListingBot is a bot for browing a catalog of job offerings. It starts by asking for a top-level refinement, a useful things to do in order to save
users from an initial open-ended interation with the bot where they don't know what they can say.

All samples target a shared, ready-to-use Azure Search service, so you don't need to provision your own to try these out. 
