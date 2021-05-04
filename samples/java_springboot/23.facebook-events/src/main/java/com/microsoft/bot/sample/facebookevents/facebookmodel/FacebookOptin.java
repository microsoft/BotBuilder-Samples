// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.facebookevents.facebookmodel;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * A Facebook optin event payload definition.
 *
 * See https://developers.facebook.com/docs/messenger-platform/reference/webhook-events/messaging_optins/
 * for more information on messaging_optin.
 */
public class FacebookOptin {

    @JsonProperty(value = "ref")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String ref;

    @JsonProperty(value = "user_ref")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String userRef;

    /**
     * Gets the optin data ref.
     * @return the Ref value as a String.
     */
    public String getRef() {
        return this.ref;
    }

    /**
     * Sets the optin data ref.
     * @param withRef The Ref value.
     */
    public void setRef(String withRef) {
        this.ref = withRef;
    }

    /**
     * Gets the optin user ref.
     * @return the UserRef value as a String.
     */
    public String getUserRef() {
        return this.userRef;
    }

    /**
     * Sets the optin user ref.
     * @param withUserRef The UserRef value.
     */
    public void setUserRef(String withUserRef) {
        this.userRef = withUserRef;
    }

}
