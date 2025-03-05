// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.servlet;

import com.microsoft.bot.builder.Bot;
import javax.servlet.annotation.WebServlet;

/**
 * This is the Servlet that will receive incoming Channel Activity messages for
 * the Bot.
 *
 * @see EchoBot
 */
@WebServlet(name = "echo", urlPatterns = "/api/messages")
public class BotController extends ControllerBase {
    private static final long serialVersionUID = 1L;

    @Override
    protected Bot getBot() {
        return new EchoBot();
    }
}
