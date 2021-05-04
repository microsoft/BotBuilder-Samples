package com.microsoft.bot.sample.nlpwithdispatch;

import com.microsoft.bot.ai.luis.LuisRecognizer;
import com.microsoft.bot.ai.qna.QnAMaker;

public interface BotServices {

    LuisRecognizer getDispatch();

    QnAMaker getSampleQnA();

}
