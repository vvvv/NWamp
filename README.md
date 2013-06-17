NWamp
=====

.NET WebSocket Application Messaging Protocol implementation (Alpha version - 0.1)

### What has been implemented so far:
 - WAMP server-side listener (with Alchemy web sockets as transport library and JSON.NET as data serializer)
 - session initiation
 - Prefixes (they are remembered as client specyfic data, yet still it's not safe to use them from client-side)
 - PubSub pattern (Unsubscribe needs to be tested more)
 - RPC calls (they may still be problems, when using RPC exstension methods registration)
 
### How to use it

Real life implementation could be found in NWamp.Alchemy project. Server side listener is represented by *AlchemyWampListener* class. Sample code could looks like this:

	var listener = new AlchemyWampListener(			
		IPAddress.Any, 								// listen on ip address
		9000, 										// listening port
		JsonConvert.SerializeObject,				// serialization delegate
		JsonConvert.DeserializeObject<object[]>);	// deserialization delegate
	listener.RegisterFunc<string,string>("http://localhost/example/hello#resp", x => "Hello, " + x);
    listener.Listen();
	
	// don't forget to dispose listener at the end of the work
	listener.Dispose();
	
Documentation will be available soon, when all main protocol paradigms will be implemented and tested.