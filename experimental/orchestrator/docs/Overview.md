# Technical Overview

The Orchestrator is a replacement of the [Bot Framework Dispatcher][1] used in chat bots since 2018. It makes the state of the art natural language understanding methods available to bot developers while at the same time making the process of language modeling quick, not requiring the expertise in [Deep Neural Networks (DNN), Transformers][5], or [Natural Language Processing (NLP)][6]. This work is co-authored with the industry experts in the field and include some of the top methods used in the [General Language Understanding Evaluation (GLUE)][7] leaderboard. Orchestrator will continue evolve and adopt the latest advancements in science and in the industry.

Orchestrator enables composability of bots allowing reuse of skills or the entire bots contributed by the community in an easy way without requiring time consuming retraining of the language models. It is our goal to support the community and continue responding to the provided feedback.

## Design Objectives and Highlights

Thanks to the community feedback we compiled a list of objectives and requirements which are addressed in the initial release of the Orchestrator. The [Roadmap](###roadmap) section describes the additional work planned in the upcoming releases. 

#### No [ML][12] or [NLP][6] expertise required

In the legacy approach so far in order to train a robust language model a significant expertise and time was required to produce a robust model. E.g. the chat bot author would be concerned with proper data distributions, data imbalance, feature-level concerns including generation of various synonym lists etc. When not paying attention to these aspects the final model quality was often poor. With the Orchestrator these aspects are of no concern anymore to the developer and the related expertise is also not required in order to create robust language model (see [Evaluation of Orchestrator on SNIPS](####evaluation-of-orchestrator-on-snips) in the advanced topics section for the evaluation results).

#### Minimal or no model training required

Building a language model requires multiple iterations of adding or removing training examples followed by training the model and evaluation. This process may take days or even weeks to accomplish satisfactory results. Also, when using the [transformer][5] model for the classification task a classification layer (or layers) are added and trained making this process expensive, time consuming and often requiring GPU.

To address these concerns, we chose an example-based approach where the language model is defined as a set of labeled examples. In Orchestrator a model example is represented as a vector of numbers (an embedding) obtained from the [transformer model][5] for a given text that the corresponding skills is capable of handling (that's the definition of the application language model in Orchestrator). During runtime a similarity of the new example is calculated comparing it to the existing model examples per skill. The weighted average of *K* closest examples ([KNN algorithm][9]) is taken to determine the classification result. This approach does not require an explicit training step, only calculation of the embeddings for the model examples is done. It takes about 10 milliseconds per example to accomplish that, so a modification of an existing model that adds 100 new examples will take about 1 second which is done locally without GPU and without remote server roundtrips.

#### Local, fast library not a remote service

The Orchestrator core is written in C++ and is available as a library in C#, Node.js and soon Python and Java. The library can be used directly by the bot code (a preferred approach) or can be hosted out-of-proc or on a remote server. Loading the English pretrained language model released for the initial preview takes about 2 sec with the memory footprint of a little over 200MB. Classification of a new example with this initial model takes about 10 milliseconds (depending on the text length). These numbers are for illustration only to give a sense of performance. As we improve the models or include additional languages these numbers will likely change.

#### State-of-the-art classification with few training examples

Developers often face an issue of a very few training examples available to properly define the language model. With the powerful pre-trained SOTA models used by the Orchestrator this is not a concern anymore. Even just one example for an intent/skill can often go a long way in making quite accurate predictions. For example, a "Greeting" intent defined with just one example, "hello", can be successfully predicted for examples like "how are you today" or "good morning to you". The power of the pretrained models and their generalization capabilities using a very few simple (and short) examples is impressive. This ability is often called a "few-shot learning" including ["one-shot learning"][11] that the Orchestrator also supports. This ability is made possible thanks to the pretrained models that were trained on large data sets and then optimized for conversation also on large data.

#### Ability to classify the "unknown" intent without additional examples

Another common challenge that developers face in handling intent classification decisions is determining whether the top scoring intent should be triggered or not. Orchestrator provides a solution for this. Its scores can be interpreted as probabilities calibrated in such way that the score of 0.5 is defined as the maximum score for an "unknown" intent selected in a way to balance the precision and recall. If the top intent's score is 0.5 or lower the query/request should be considered of an "unknown" intent and should probably trigger a follow up question by the bot. On the other hand, if the score of two intents is above 0.5 then both intents (skills) could be triggered. If the bot is designed to handle only one intent at a time, then the application rules or other priorities could pick the one that gets triggered in this case.

The classification of the "unknown" intent is done without the need for any examples that define the "unknown" (often referred to as ["zero-shot learning"][10]) which would be challenging to accomplish. It would be hard to accomplish this without the heavily pretrained language model especially that the bot application may be extended in the future with additional skills that were "unknown" so far.

#### Extend to support Bot Builder Skills

While the [Dispatcher's][1] focus was to aid in triggering between multiple [LUIS][3] apps and [QnA Maker][4] KBs the Orchestrator expands this functionality into supporting generic [Bot Builder Skills][2] to allow composability of bot skills. The skills developed and made available by the community may be easily reused and integrated in a new bot with no language model retraining required. Orchestrator provides a toolkit to evaluate this extension identifying ambiguous examples that should be reviewed by the developer. Also, an optional fine-tuning CLI will be made available in future releases but this step is not required in most cases.

#### Ease of composability

The language models of skills and even entire bots that are made available by the community can be integrated in a new bot by simply adding their snapshot(s) (see the [API reference][20] for more information). Model snapshots represent skills, group of skills or even entire bots, contain all the language model data required to trigger them. Importing a new model snapshot can be done in runtime and takes just milliseconds. This opens opportunities for interesting scenarios where the model can be modified to emphasize deeper, more specialized skills that are likely to trigger. This flexibility is beneficial in cases of complex dialogs or even for handling the conversation contexts which could include model snapshots.

#### Ability to explain the classification results

The ability to explain classification results could be important in an application. In general, attempting to interpret the results of deep learned models (like [transformers][5]) could be very challenging. Orchestrator enables this by providing the closest example in the model to the one that is evaluated. In a case of misclassification this simple mechanism helps the developer in determining whether a new example should be added that defines a skill or if the existing example in the model was mislabeled. This feature simplifies implementation of [reinforcement learning][18] for the bot which can be done by non-experts (the language fluency is only required).

#### High performance

The core of Orchestrator is written in C++. Since its runtime algorithms can be easily vectorized the Orchestrator core takes advantage of the vector operators supported by the mainstream CPUs ([SIMD][13]) without the need for a [GPU][14]. As a result, similarity calculation time during [KNN][9] inference is negligible comparing with other local processing tasks even for largest models.

#### Compact models

The [transformer][5] models in Orchestrator produce embeddings that are relatively large, over 3kB in size per example (size of the embeddings). If these large embeddings were used directly not only this would increase the runtime memory requirement quite significantly but also would add substantial CPU processing costs. A commonly used similarity measure, cosine similarity, with this size of embeddings and KNN processing would add a significant overhead during inference. Instead of this approach, Orchestrator uses a quantization method that shrinks the embeddings to under 100 bytes in size, reducing the processing time over 50 times while preserving the same level of accuracy. This technology is available already in the initial public preview of Orchestrator. 

#### Runtime flexibility

It is important to reiterate that the Orchestrator runtime has significantly more flexibility and functionality than a typical [transformer][5] or a generic [ML][12] runtime. In addition to the inference capability the developer has an option to enable the following in the bot code:

*Modify the language model in real-time* - to add additional functionality (expand the language model with additional skills or examples) or perform continuous model improvements using [reinforcement learning][18] techniques (specialized tools to assist with reinforcement learning will be released in the upcoming releases).

*Modify the language model behavior in real-time* - the runtime parameters can be adjusted without restarting the process or even reloading the model. This includes adjusting how strict the intent triggering is (tradeoff between the [precision and recall][19]) which can be dynamically adjusted depending on the phase in the dialog; or adjusting the resiliency to mislabeled or low quality examples that define the model which is done by modifying the KNN-K value (e.g. a case where the model examples were crowd-sourced and not cleaned up yet or when the model is allowed to be adjusted dynamically by many people or when a skill language model definition was added to the bot and not evaluated yet).

## Roadmap

In the upcoming releases we are planning to expand Orchestrator in several areas:
 
***Entity recognition***

A commonly requested feature as the part of intent triggering is to provide the "parameters" for the triggered intents which are entities recognized in the query text. The Orchestrator interfaces which are already part of the initial preview support handling the recognized entities. This functionality together with the corresponding prebuilt language model(s) will be made available in the upcoming releases.

***Multi-lingual models***

An important extension that will be made in the upcoming releases is the support for multi-lingual models and possibly also specialized international models prioritized by languages supported by other Microsoft offerings.

***Extensibility with custom pretrained language models***

The prebuilt language models' format and the runtime supported for the initial release is [ONNX][15]. We will extend Orchestrator to directly support [PyTorch][16] and [TensorFlow][17] model formats and their corresponding runtimes.

***Reinforcement learning***

The Orchestrator design with its [flexibility](###runtime-flexibility) provides capability for efficient [reinforcement learning][18] for continuous language model improvements. Additional tools for this purpose to assist with this task and help in its automation will be released in the upcoming releases.

**Expand model tuning capability** - currently all the model parameters (hyper-params) are global for all intents/skills. In the upcoming releases the configuration per intent will be enabled. E.g. for certain intents the triggering should be more strict and for other ones more fuzzy or even with a catch-all type of behavior on the language model level ([precision vs recall][19] control per intent).



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
[15]:https://onnx.ai/
[16]:https://en.wikipedia.org/wiki/PyTorch
[17]:https://en.wikipedia.org/wiki/TensorFlow
[18]:https://en.wikipedia.org/wiki/Reinforcement_learning
[19]:https://en.wikipedia.org/wiki/Precision_and_recall
[20]:https://github.com/microsoft/BotBuilder-Samples/blob/vishwac/r10/orchestrator/experimental/orchestrator/docs/API_reference.md



