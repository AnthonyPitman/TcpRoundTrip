# TcpRoundTrip

**Week 3** - Networking: Build a TCP message round trip
**Time**: 30-45 minutes
**Dependencies**: .NET SDK; no external packages.

Build a console app with two modes: a small TCP server and an interactive client. This exercise stands alone; previous sessions aren't prerequisites.

## Code task

Run the app in two terminals

```sh
dotnet run -- server
dotnet run --client
```

The server should bind to `IPAddress.Loopback` on port `5050`, accept one client, and process messages until that client disconnects. The client should connect to that address and read messages from the console.

Using a simple protocol: each message is one UTF-8 line, and each response is one UTF-8 line containing `ACK:` followed by the original message.

```text
Client sends: hello
Server responds: ACK: hello
```

Use `TcpListener`, `TcpClient`, `StreamReader`, and `StreamWriter`, with asynchronous network operations. Set he writer's `AutoFlush` property to `true`. Send one message and await its response before accepting the next console input. Entering `/quit` should close the client locally without sending that command.

Keep the scope to one connected client and short, single-line messages. Dispose connections and streams when finished, and show a clear message if the server is unavailable.

## Acceptance criteria

- Sending `hello`, an empty line, and `café` produces the corresponding acknowledgement for each.
- Several messages work over the same connection.
- `/quit` closes the client, and the server ditects the disconnect and exits without spinning in a loop.

## Underlying concept

TCP provides an ordered stream of bytes, without preserving application message boundaries. One write may require several reads; several writes may arrive together. Your protocol therefore needs **framing**: a rule defining where each message ends. Here, newline delimiters and `ReadLineAsync` provide that rule. A `null` read result indicates the peer has ended its sending stream.

**Hint**: Both sides must agree on encoding, framing, and when to flush. A response still sitting in a writer's buffer can make a healthy connection appear stuck.

**Stretch question**: How would you redesign the framing is a message could contain embedded newlines?
