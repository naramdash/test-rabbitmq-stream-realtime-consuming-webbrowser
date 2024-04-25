import { HubConnectionBuilder } from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";

const connection = new HubConnectionBuilder()
  .withUrl(`http://localhost:5011/signalr`)
  .withHubProtocol(new MessagePackHubProtocol())
  .withAutomaticReconnect()
  .build();

connection.on("Pub", (msg) => {
  self.postMessage({ vhl: msg.vhl, tick: msg.tick });
});

connection.start();
