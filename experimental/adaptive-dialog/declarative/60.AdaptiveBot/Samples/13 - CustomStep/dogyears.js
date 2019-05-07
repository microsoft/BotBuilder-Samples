function doStep(user, conversation, dialog, turn) {
    // inputBindings are passed in as dialog.result
    // so an input binding of "age" comes in as dialog.result.age
    return dialog.result.age * 7;
}