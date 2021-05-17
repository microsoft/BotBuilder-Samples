// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.dialogrootbot.controller;

import com.microsoft.bot.builder.ChannelServiceHandler;
import com.microsoft.bot.integration.spring.ChannelServiceController;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping(value = {"/api/skills"})
public class SkillController extends ChannelServiceController {

    public SkillController(ChannelServiceHandler handler) {
        super(handler);
    }
}
