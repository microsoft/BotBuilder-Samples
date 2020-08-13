# Technical Overview

The Orchestrator is a replacement of the [Bot Framework Dispatcher][1] used in chat bots since 2018. It makes the state of the art natural language understanding methods available to the bot developers while at the same time making the process of language modeling quick, not requiring expertise in [Deep Neural Networks (DNN), Transformers][5], or [Natural Language Processing (NLP)][6] required. This work is co-authored with industry experts in the field and include some of the top methods of the [General Language Understanding Evaluation (GLUE)][7] leaderboard. Orchestrator also enables composability of bots allowing reuse of skills or the entire bots contributed by the community in an easy way without requiring time consuming retraining of the language models.

## Design Objectives and Requirements

Thanks to the community feedback we compiled a list of objectives and requirements which are addressed by the initial release of the Orchestrator.

### No Machine Learning or [NLP][6] expertise required

In the legacy approach so far in order to train a robust language model a significant expertise and time was required. E.g. the chat bot author would be concerned with proper data distributions, data imbalance, feature-level concerns including generation of various synonym lists etc. Without these the final classification results were often poor. With the Orchestrator these aspects are of no concern anymore to the developer and the related expertise is also not required in order to create robust language model (see [Evaluation of Orchestrator on SNIPS](###evaluation-of-orchestrator-on-snips) in the advanced topics section for the evaluation results).

### Minimal or no model training

*Content coming soon*

### Local, fast library not a remote service

*Content coming soon*

### State-of-the-art classification with very few training examples

*Content coming soon*

### Extend to support Bot Builder Skills

While the [Dispatcher's][1] focus was to aid in triggering between multiple [LUIS][3] apps and [QnA Maker][4] KBs the Orchestrator expands this functionality into supporting generic [Bot Builder Skills][2] to allow composability of bot skills. The skills developed and made available by the community may now be easily integrated in your chat bot with no language model retraining. 

### Ability to classify the "unknown" intent without training examples

*Content coming soon*

### Ease of composability

*Content coming soon*

### Ability to explain the classification results

*Content coming soon*

### High performance

## Approach

*Content coming soon*

### Example-based (instance-based) approach

*Content coming soon*

### Optimized for conversation

*Content coming soon*

### Compact models

*Content coming soon*

### Runtime flexibility

*Content coming soon*

## Advanced Topics

*Content coming soon*

### Multi-intent (multi-label) support

*Content coming soon*

### Ease of controlling precision vs recall

*Content coming soon*

### Composability

*Content coming soon*

### Evaluation of Orchestrator on SNIPS

We evaluated Orchestrator using the [SNIPS data][8] comparing with other common SOTA approaches.

*Content coming soon*

[1]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-tutorial-dispatch?view=azure-bot-service-4.0&tabs=cs
[2]:https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-skills-overview?view=azure-bot-service-4.0
[3]:https://www.luis.ai/
[4]:https://www.qnamaker.ai/
[5]:https://en.wikipedia.org/wiki/Transformer_(machine_learning_model)
[6]:https://en.wikipedia.org/wiki/Natural_language_processing
[7]:https://gluebenchmark.com/leaderboard
[8]:https://github.com/snipsco/nlu-benchmark


