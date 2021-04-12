// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.core;

/**
 * The model class to retrieve the information of the booking.
 */
public class BookingDetails {

    private String destination;
    private String origin;
    private String travelDate;

    /**
     * Gets the destination of the booking.
     *
     * @return The destination.
     */
    public String getDestination() {
        return destination;
    }


    /**
     * Sets the destination of the booking.
     *
     * @param withDestination The new destination.
     */
    public void setDestination(String withDestination) {
        this.destination = withDestination;
    }

    /**
     * Gets the origin of the booking.
     *
     * @return The origin.
     */
    public String getOrigin() {
        return origin;
    }

    /**
     * Sets the origin of the booking.
     *
     * @param withOrigin The new origin.
     */
    public void setOrigin(String withOrigin) {
        this.origin = withOrigin;
    }

    /**
     * Gets the travel date of the booking.
     *
     * @return The travel date.
     */
    public String getTravelDate() {
        return travelDate;
    }

    /**
     * Sets the travel date of the booking.
     *
     * @param withTravelDate The new travel date.
     */
    public void setTravelDate(String withTravelDate) {
        this.travelDate = withTravelDate;
    }
}
