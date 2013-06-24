NWamp
=====

.NET [WebSocket Application Messaging Protocol](http://wamp.ws/) server implementation. Current branch is [0.1-beta](https://github.com/Horusiath/NWamp/tree/0.1-beta). Current implementation supports both RPC and Pub/Sub patterns.

Since this is a beta version of the library, future API may change. See the docs for more info.

##	Roadmap

-	Change internal implementation - modularize the code and bring on unit tests. This version will be continued on separate branch and may result in unification of event delegate methods.
-	Extends event parameter fields - there are more usefull properties which could be added to event args objects.
-	Optimize performance.
-	SignalR web socket implementation.
