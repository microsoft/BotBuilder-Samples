// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.scaleout;

import com.microsoft.bot.builder.StatePropertyAccessor;
import com.microsoft.bot.builder.TurnContext;

import java.util.concurrent.CompletableFuture;
import java.util.function.Supplier;

/**
 * This is an accessor for any object. By definition objects (as opposed to values)
 * are returned by reference in the get call on the accessor. As such the set
 * call is never used. The actual act of saving any state to an external store therefore
 * cannot be encapsulated in the Accessor implementation itself. And so to facilitate this
 * the state itself is available as a public property on this class. The reason its here is
 * because the caller of the constructor could pass in null for the state, in which case
 * the factory provided on the get call will be used.
 * @param <T> The value type of the RefAccessor class.
 */
public class RefAccessor<T> implements StatePropertyAccessor<T> {
    private T value;

    /**
     * Sets the value object.
     * @param withValue The specified new value.
     */
    public RefAccessor(T withValue) {

        value = withValue;
    }

    /**
     * Gets the value object.
     * @return The value object.
     */
    public T getValue() {
        return value;
    }

    /**
     * Gets the TypeName of the value's class.
     * @return The String representing the TypeName of the value's class.
     */
    public String getName() {
        return value.getClass().getTypeName();
    }

    /**
     * Gets the value object.
     * @param turnContext Context object containing information for a single turn of conversation with a user.
     * @param defaultValueFactory The defaultValueFactory object.
     * @return The CompletableFuture representing the value object.
     */
    public CompletableFuture<T> get(TurnContext turnContext, Supplier<T> defaultValueFactory) {
        if (value == null) {
            if (defaultValueFactory == null) {
                throw new IllegalArgumentException("defaultValueFactory cannot be null");
            }

            value = defaultValueFactory.get();
        }

        return CompletableFuture.completedFuture(value);
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<Void> delete(TurnContext turnContext) {
        throw new UnsupportedOperationException();
    }

    /**
     * {@inheritDoc}
     */
    @Override
    public CompletableFuture<Void> set(TurnContext turnContext, T value) {
        throw new UnsupportedOperationException();
    }

    private void setValue(T withValue) {
        this.value = withValue;
    }
}
