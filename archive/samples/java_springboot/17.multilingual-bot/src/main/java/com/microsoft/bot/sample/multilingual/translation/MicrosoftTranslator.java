// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.multilingual.translation;

import java.io.Reader;
import java.io.StringReader;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.CompletionException;

import com.microsoft.bot.integration.Configuration;
import com.microsoft.bot.sample.multilingual.translation.model.TranslatorResponse;

import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.common.base.Strings;

import org.slf4j.LoggerFactory;

/**
 * A helper class wrapper for the Microsoft Translator API.
 */
public class MicrosoftTranslator {
    private static final String HOST = "https://api.cognitive.microsofttranslator.com";
    private static final String PATH = "/translate?api-version=3.0";
    private static final String URI_PARAMS = "&to=";

    private final String key;

    /**
     * @param configuration The configuration class with the translator key stored.
     */
    public MicrosoftTranslator(Configuration configuration) {
        String translatorKey = configuration.getProperty("TranslatorKey");

        if (translatorKey == null) {
            throw new IllegalArgumentException("key");
        }

        key = translatorKey;
    }

    /**
     * Helper method to translate text to a specified language.
     * @param text Text that will be translated.
     * @param targetLocale targetLocale Two character language code, e.g. "en", "es".
     * @return The first translation result
     */
    public CompletableFuture<String> translate(String text, String targetLocale) {
        return CompletableFuture.supplyAsync(() -> {
            // From Cognitive Services translation documentation:
            // https://docs.microsoft.com/en-us/azure/cognitive-services/Translator/quickstart-translator?tabs=java
            String body = String.format("[{ \"Text\": \"%s\" }]", text);

            String uri = new StringBuilder(MicrosoftTranslator.HOST)
                .append(MicrosoftTranslator.PATH)
                .append(MicrosoftTranslator.URI_PARAMS)
                .append(targetLocale).toString();

            RequestBody requestBody = RequestBody.create(MediaType.parse("application/json"), body);

            OkHttpClient client = new OkHttpClient();
            Request request = new Request.Builder()
                .url(uri)
                .header("Ocp-Apim-Subscription-Key", key)
                .post(requestBody)
                .build();

            try {
                Response response = client.newCall(request).execute();

                if (!response.isSuccessful()) {
                    String message = new StringBuilder("The call to the translation service returned HTTP status code ")
                        .append(response.code())
                        .append(".").toString();
                    throw new Exception(message);
                }

                ObjectMapper objectMapper = new ObjectMapper();
                Reader reader = new StringReader(response.body().string());
                TranslatorResponse[] result = objectMapper.readValue(reader, TranslatorResponse[].class);

                return result[0].getTranslations().get(0).getText();

            } catch (Exception e) {
                LoggerFactory.getLogger(MicrosoftTranslator.class).error("findPackages", e);
                throw new CompletionException(e);
            }
        });
    }
}
