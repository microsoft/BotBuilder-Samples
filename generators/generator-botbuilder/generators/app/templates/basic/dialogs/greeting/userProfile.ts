// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Simple object used by the user state property accessor.
 * Used to store the user state.
 */
class UserProfile {
  // member variables
  public name: string;
  public city: string;
  /**
   * Constructor. Members initialized with undefined,
   *  if no values provided via constructor
   *
   * @param name string
   * @param city string
   */
  constructor(name?: string, city?: string) {
    this.name = name || undefined;
    this.city = city || undefined;
  }
}

export { UserProfile };
