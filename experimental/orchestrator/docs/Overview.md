# Technical Overview

The Orchestrator is a replacement of the [Bot Framework Dispatcher][1] used in chat bots since 2018. It makes the state of the art natural language understanding methods available to bot developers while at the same time making the process of language modeling quick, not requiring the expertise in [Deep Neural Networks (DNN), Transformers][5], or [Natural Language Processing (NLP)][6]. This work is co-authored with industry experts in the field and include some of the top methods of the [General Language Understanding Evaluation (GLUE)][7] leaderboard. Orchestrator also enables composability of bots allowing reuse of skills or the entire bots contributed by the community in an easy way without requiring time consuming retraining of the language models.

## Design Objectives and Highlights

Thanks to the community feedback we compiled a list of objectives and requirements which are addressed in the initial release of the Orchestrator. The [Roadmap](###roadmap) section describes the additional work planned in the upcoming releases. 

### No [ML][12] or [NLP][6] expertise required

In the legacy approach so far in order to train a robust language model a significant expertise and time was required. E.g. the chat bot author would be concerned with proper data distributions, data imbalance, feature-level concerns including generation of various synonym lists etc. Without these the final classification results were often poor. With the Orchestrator these aspects are of no concern anymore to the developer and the related expertise is also not required in order to create robust language model (see [Evaluation of Orchestrator on SNIPS](###evaluation-of-orchestrator-on-snips) in the advanced topics section for the evaluation results).

### Minimal or no model training

Building a language model requires multiple iterations of adding or removing training examples followed by training the model and evaluation. This is often done multiple times and may take days or even weeks. When using the [transformer][5] model for the classification task a classification layer (or layers) are added and trained making this process expensive, time consuming and often requiring GPU. To address this we chose an example-based approach instead of adding a classification layer. In Orchestrator an example (text example that represents a skill or intent) is represented as a vector of numbers (an embedding) obtained from the transformer model for a given example text. During runtime a similarity of the new example is calculated comparing to the existing embeddings that define the model taking weighted average of *K* closest examples ([KNN algorithm][9]). This approach does not require an explicit training step beyond calculating the embeddings for the examples that define the model. It takes about 10 milliseconds per example, so a change to the model that adds 100 examples will take about 1 second which is done locally without GPU and without remote server round-trips.

### Local, fast library not a remote service

The Orchestrator is written in C++ and is available as a library in C#, Node.js and soon Python and Java. The library can be used directly by the bot code or hosted out-of-proc. Loading the generic pretrained language model takes less then 2 sec with the memory footprint of a little over 200MB. 

### State-of-the-art classification with very few training examples

Bot developers often face lack of training data to properly define the language model. With the powerful pre-trained SOTA models used by the Orchestrator this is not a concern anymore. Even just one example for an intent/skill can often go a long way in making accurate predictions. For example a "Greeting" intent defined with just one example, "hello", can be successfully predicted for examples like "how are you today" or "good morning to you" etc. The power of the pretrained models and their generalization capabilities done from a very few simple (and short) examples is impressive. This ability is often called a "few-shot learning" including ["one-shot learning"][11] that the Orchestrator also supports. This ability is made possible thanks to the models that were pretrained on a large data sets.

### Ability to classify the "unknown" intent without additional examples

Another common challenge that developers face during classification of intents is determining whether the top scoring intent should be triggered or not. Orchestrator provides a solution for this. Its scores can be interpreted as probabilities calibrated in such way that the score of 0.5 is defined as the maximum score for an "unknown" intent. So if the top intent's score is 0.5 or lower the query/request should be considered of an "unknown" intent and should probably trigger a follow up question by the bot. On the other hand if the score of two intents is above 0.5 then both intents (skills) could be triggered. If the bot is designed to handle only one intent at a time then the application rules or other priorities could pick the one that gets triggered.

The classification of the "unknown" intent is done without the need for any examples for that purpose (["zero-shot learning"][10]) which would be hard to accomplish especially that the model may be extended in the future with additional skills that were "unknown" so far.

### Extend to support Bot Builder Skills

While the [Dispatcher's][1] focus was to aid in triggering between multiple [LUIS][3] apps and [QnA Maker][4] KBs the Orchestrator expands this functionality into supporting generic [Bot Builder Skills][2] to allow composability of bot skills. The skills developed and made available by the community may be easily reused and integrated in a new bot with no language model retraining required. An optional fine-tuning CLI will be made available but this step is not required in most cases.

### Ease of composability

The language models of skills and even the entire bots that made available by the community can be integrated in a new bot by simply adding their snapshot(s). Model snapshots represent skills, group of skills or even entire bots, contain all the language model data required to trigger them. Import of a new snapshot can be done in runtime and takes just milliseconds. This opens opportunities for interesting scenarios where the model can be dynamically modified to emphasize deeper, more specialized skills that are likely to trigger. This flexibility is beneficial in cases of complex dialogs or even the conversation context which could capture the model snapshots.

### Ability to explain the classification results

The ability to explain the classification results could be important but attempting to interpret the results of deep learned models (like [transformers][5]) could be very challenging. Orchestrator enables this by providing the closest model example to the one that is evaluated. In case of misclassification this simple mechanism helps in determining whether a new example should be added to expand the bot language model or if the existing example in the model was mislabeled. This feature simplifies implementation of reinforcement learning for the bot which can be done by non-experts (the language fluency is only required).

### High performance

The core of Orchestrator is written in C++ and its runtime algorithms can be easily vectorized takes advantage of the vector operators provided by the mainstream CPUs ([SIMD][13]) without the need for [GPU][14]. As a result, calculating similarity during inference takes about 10 milliseconds in a model with 1 million examples (typically language models for bots have much fewer number examples, often just hundreds).

### Compact models

*Content coming soon*

### Runtime flexibility

*Content coming soon*

## Roadmap

*Content coming soon*

## Advanced Topics

*Content coming soon*

### Multi-intent (multi-label) support

*Content coming soon*

### Ease of controlling precision vs recall

*Content coming soon*

### Bot language model composability

*Content coming soon*

### Evaluation of Orchestrator on SNIPS

We evaluated Orchestrator using the [SNIPS data][8] comparing with other common SOTA approaches.

*Content coming soon*

### Optional runtime configuration parameters

[1]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=cs
[2]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-skills-overview?view=azure-bot-service-4.0
[3]:https://www.luis.ai/
[4]:https://www.qnamaker.ai/
[5]:https://en.wikipedia.org/wiki/Transformer_(machine_learning_model)
[6]:https://en.wikipedia.org/wiki/Natural_language_processing
[7]:https://gluebenchmark.com/leaderboard
[8]:https://github.com/snipsco/nlu-benchmark
[9]:https://en.wikipedia.org/wiki/K-nearest_neighbors_algorithm
[10]:https://en.wikipedia.org/wiki/Zero-shot_learning
[11]:https://en.wikipedia.org/wiki/One-shot_learning
[12]:https://en.wikipedia.org/wiki/Machine_learning
[13]:https://en.wikipedia.org/wiki/SIMD
[14]:https://en.wikipedia.org/wiki/General-purpose_computing_on_graphics_processing_units
