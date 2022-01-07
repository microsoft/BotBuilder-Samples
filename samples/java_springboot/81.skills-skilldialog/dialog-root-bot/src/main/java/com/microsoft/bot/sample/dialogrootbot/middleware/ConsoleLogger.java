// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogrootbot.middleware;

import java.util.concurrent.CompletableFuture;

public class ConsoleLogger extends Logger {

    @Override
    public CompletableFuture<Void> logEntry(String entryToLog) {
        return super.logEntry(entryToLog);
    }
}
