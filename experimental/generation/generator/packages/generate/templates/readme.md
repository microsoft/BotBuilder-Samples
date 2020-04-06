# Organization
Every property has:
* ${schema}-${property}.${locale}.lg -- localized lg file.
* ${schema}-${property}-missing.dialog -- Ask for missing property.
* ${schema}-${property}-clear.dialog -- Clear property.
* ${schema}-${property}-show.dialog -- Show the current property value.

For each entity found in a property:
* ${schema}-${property}-${entity}.${locale}.lu -- Definition of entity for a particular property.
* ${schema}-${property}-add-${entity}.dialog -- Add an entity to property.
* ${schema}-${property}-remove-${entity}.dialog -- Remove an entity from property.
* 
* ${gets some standard templates:
* ${property}Name(property) -- how to describe the property in human terms
* ${property}(val) -- display a property value

The templates make use of a generation time 