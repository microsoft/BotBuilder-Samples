# RemoteDialog
An example of how to call a remote dialog.

The Dialog here is modeled as a function, this maps simply to an HTTP POST. Swap out the MyDialog.ProcessAsync function implementation for one that makes an HTTP POST call to a service. The service can be stateless because on each invocation it takes the previous old state and returns the new state.

