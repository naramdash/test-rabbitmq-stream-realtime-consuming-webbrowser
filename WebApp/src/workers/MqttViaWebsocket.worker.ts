import mqtt from "mqtt";

const client = mqtt.connect("ws://localhost:15675/ws");
client.on("connect", function () {
  client.subscribe("iot/sensor/v34/+/status", function (err) {
    if (!err) console.log("Subscribed to iot/sensor/v34/+/status");
  });
  console.log("Connected to MQTT broker");
});
client.on("message", function (topic, message) {
  const msg = JSON.parse(message.toString()) as { vhl: string; tick: number };
  self.postMessage({ vhl: msg.vhl, tick: msg.tick });
});
