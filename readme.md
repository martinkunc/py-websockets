Proof of concept of websocket communication between python and .net with simple performace measurement.


# Requirements

pip install asyncio
pip install websockets

## Run .net server
 Start-Process dotnet { run --project ./net-ws-server/ }




## Performance:
Is about 2000 data exchanges per second, that is about 0.03 s per message.

See [perflog.txt](perflog.txt) for full log from server with perf data.
```
HandleWs: Debug: Exchanges per second:2188
dbug: HandleWs[0]
      Exchanges per second:2184
HandleWs: Debug: Exchanges per second:2184
dbug: HandleWs[0]
      Exchanges per second:2211
HandleWs: Debug: Exchanges per second:2211
dbug: HandleWs[0]
      Exchanges per second:2203
HandleWs: Debug: Exchanges per second:2203
dbug: HandleWs[0]
      Exchanges per second:2190
HandleWs: Debug: Exchanges per second:2190
dbug: HandleWs[0]
      Exchanges per second:1978
HandleWs: Debug: Exchanges per second:1978
```

