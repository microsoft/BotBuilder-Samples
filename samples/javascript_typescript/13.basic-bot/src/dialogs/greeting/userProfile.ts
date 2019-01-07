// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

/**
 * Simple object used by the user state property accessor.
 * Used to store the user state.
 */
class UserProfile {
    // member variables
    public name: string;
<<<<<<< HEAD
    public city: string; 
    /**
     * Constructor. Members initialized with undefined,
     *  if no values provided via constructor
     * 
=======
    public city: string;
    /**
     * Constructor. Members initialized with undefined,
     *  if no values provided via constructor
     *
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
     * @param name string
     * @param city string
     */
    constructor(name?: string, city?: string) {
      this.name = name || undefined;
      this.city = city || undefined;
    }
<<<<<<< HEAD
  };  
  
export { UserProfile };
=======
  }

export { UserProfile };
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
