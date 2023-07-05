import asyncio
import websockets
import logging
import time

log = logging.getLogger(__name__)

logHandler = logging.StreamHandler()
logHandler.setLevel(logging.DEBUG)
log.addHandler(logHandler)

async def hello():    
    """ Connects to remote WS server and continuously sends Hello and prints received responses
    """
    async with websockets.connect("ws://localhost:5001/ws") as websocket:
        while True:
            try:
                start = time.time()
                await websocket.send("Hello")
                #print(f"send took: {time.time() - start}")
                start = time.time()
                message = await websocket.recv()
                #print(f"receive took :{time.time() - start}")
                #print(f"Received: {message}")
                # To test slow client
                # await asyncio.sleep(1)
            except websockets.exceptions.ConnectionClosedError:
                log.debug("Connection closed")
                break
            except Exception as e:
                log.warning("Error during communication" + str(e))
        
    
def main():
    #logging.basicConfig(level=logging.DEBUG)
    #log.setLevel(logging.DEBUG)
    
    asyncio.run(hello())

if __name__ == '__main__':
   main()