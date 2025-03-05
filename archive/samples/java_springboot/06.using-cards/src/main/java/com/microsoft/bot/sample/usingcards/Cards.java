// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.usingcards;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.microsoft.bot.schema.ActionTypes;
import com.microsoft.bot.schema.AnimationCard;
import com.microsoft.bot.schema.Attachment;
import com.microsoft.bot.schema.AudioCard;
import com.microsoft.bot.schema.CardAction;
import com.microsoft.bot.schema.CardImage;
import com.microsoft.bot.schema.Fact;
import com.microsoft.bot.schema.HeroCard;
import com.microsoft.bot.schema.MediaUrl;
import com.microsoft.bot.schema.OAuthCard;
import com.microsoft.bot.schema.ReceiptCard;
import com.microsoft.bot.schema.ReceiptItem;
import com.microsoft.bot.schema.SigninCard;
import com.microsoft.bot.schema.ThumbnailCard;
import com.microsoft.bot.schema.ThumbnailUrl;
import com.microsoft.bot.schema.VideoCard;
import java.io.InputStream;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.CompletionException;
import org.apache.commons.io.IOUtils;

public class Cards {
    public static Attachment createAdaptiveCardAttachment() {
        Attachment adaptiveCardAttachment = new Attachment();

        try (
            InputStream inputStream = adaptiveCardAttachment.getClass().getClassLoader()
            .getResourceAsStream("adaptiveCard.json")
        ) {
            String result = IOUtils.toString(inputStream, StandardCharsets.UTF_8);

            adaptiveCardAttachment.setContentType("application/vnd.microsoft.card.adaptive");
            adaptiveCardAttachment.setContent(new ObjectMapper().readValue(result, ObjectNode.class));

            return adaptiveCardAttachment;
        } catch (Throwable t) {
            throw new CompletionException(t);
        }
    }

    public static HeroCard getHeroCard() {
        HeroCard heroCard = new HeroCard();
        heroCard.setTitle("BotFramework Hero Card");
        heroCard.setSubtitle("Microsoft Bot Framework");
        heroCard.setText("Build and connect intelligent bots to interact with your users naturally wherever they are," +
                    " from text/sms to Skype, Slack, Office 365 mail and other popular services.");
        heroCard.setImages(new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg"));
        heroCard.setButtons(new CardAction(ActionTypes.OPEN_URL, "Get Started", "https://docs.microsoft.com/bot-framework"));

        return heroCard;
    }

    public static ThumbnailCard getThumbnailCard() {
        ThumbnailCard thumbnailCard = new ThumbnailCard();
        thumbnailCard.setTitle("BotFramework Thumbnail Card");
        thumbnailCard.setSubtitle("Microsoft Bot Framework");
        thumbnailCard.setText("Build and connect intelligent bots to interact with your users naturally wherever they are," +
                    " from text/sms to Skype, Slack, Office 365 mail and other popular services.");
        thumbnailCard.setImages(new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg"));
        thumbnailCard.setButtons(new CardAction(
            ActionTypes.OPEN_URL, "Get Started", "https://docs.microsoft.com/bot-framework"));

        return thumbnailCard;
    }

    public static ReceiptCard getReceiptCard() {
        ReceiptCard receiptCard = new ReceiptCard();
        receiptCard.setTitle("John Doe");
        ReceiptItem receiptDataTransfer = new ReceiptItem();
        receiptDataTransfer.setTitle("Data Transfer");
        receiptDataTransfer.setPrice("$ 38.45");
        receiptDataTransfer.setQuantity("368");
        receiptDataTransfer.setImage(new CardImage("https://github.com/amido/azure-vector-icons/raw/master/renders/traffic-manager.png"));
        ReceiptItem receiptAppService = new ReceiptItem();
        receiptAppService.setTitle("App Service");
        receiptAppService.setPrice("$ 45.00");
        receiptAppService.setQuantity("720");
        receiptAppService.setImage(new CardImage("https://github.com/amido/azure-vector-icons/raw/master/renders/cloud-service.png"));
        receiptCard.setFacts(new Fact("Order Number", "1234"), new Fact("Payment Method", "VISA 5555-****"));
        receiptCard.setItems(receiptDataTransfer, receiptAppService);
        receiptCard.setTax("$ 7.50");
        receiptCard.setTotal("$ 90.95");
        CardAction cardAction = new CardAction(ActionTypes.OPEN_URL, "More information");
        cardAction.setImage("https://account.windowsazure.com/content/6.10.1.38-.8225.160809-1618/aux-pre/images/offer-icon-freetrial.png");
        cardAction.setValue("https://azure.microsoft.com/en-us/pricing/");
        receiptCard.setButtons(cardAction);

        return receiptCard;
    }

    public static SigninCard getSigninCard() {
        SigninCard signinCard = new SigninCard();
        signinCard.setText("BotFramework Sign-in Card");
        signinCard.setButtons(new CardAction(ActionTypes.SIGNIN, "Sign-in", "https://login.microsoftonline.com/"));
        return signinCard;
    }

    public static AnimationCard getAnimationCard() {
        AnimationCard animationCard = new AnimationCard();
        animationCard.setTitle("Microsoft Bot Framework");
        animationCard.setSubtitle("Animation Card");
        animationCard.setImage(new ThumbnailUrl("https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png"));
        animationCard.setMedia(new MediaUrl("http://i.giphy.com/Ki55RUbOV5njy.gif"));
        return animationCard;
    }

    public static VideoCard getVideoCard() {
        VideoCard videoCard = new VideoCard();
        videoCard.setTitle("Big Buck Bunny");
        videoCard.setSubtitle("by the Blender Institute");
        videoCard.setText("Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender Institute," +
            " part of the Blender Foundation. Like the foundation's previous film Elephants Dream," +
            " the film was made using Blender, a free software application for animation made by the same foundation." +
            " It was released as an open-source film under Creative Commons License Attribution 3.0.");
        videoCard.setImage(new ThumbnailUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Big_buck_bunny_poster_big.jpg/220px-Big_buck_bunny_poster_big.jpg"));
        videoCard.setMedia(new MediaUrl("http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4"));
        videoCard.setButtons(new CardAction(ActionTypes.OPEN_URL, "Learn More", "https://peach.blender.org/"));

        return videoCard;
    }

    public static AudioCard getAudioCard() {
        AudioCard audioCard = new AudioCard();
        audioCard.setTitle("I am your father");
        audioCard.setSubtitle("Star Wars: Episode V - The Empire Strikes Back");
        audioCard.setText("The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes Back)" +
                " is a 1980 American epic space opera film directed by Irvin Kershner. Leigh Brackett and" +
                " Lawrence Kasdan wrote the screenplay, with George Lucas writing the film's story and serving" +
                " as executive producer. The second installment in the original Star Wars trilogy, it was produced" +
                " by Gary Kurtz for Lucasfilm Ltd. and stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams," +
                " Anthony Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.");
        audioCard.setImage(new ThumbnailUrl("https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg"));
        audioCard.setMedia(new MediaUrl("http://www.wavlist.com/movies/004/father.wav"));
        audioCard.setButtons(new CardAction(ActionTypes.OPEN_URL, "Read More", "https://en.wikipedia.org/wiki/The_Empire_Strikes_Back"));

        return audioCard;
    }

    public static OAuthCard getOAuthCard() {
        OAuthCard oauthCard = new OAuthCard();
        oauthCard.setText("BotFramework OAuth Card");
        oauthCard.setConnectionName("OAuth connection"); // Replace with the name of your Azure AD connection.
        oauthCard.setButtons(new CardAction(ActionTypes.SIGNIN, "Sign In", "https://example.org/signin"));
        return oauthCard;
    }
}
