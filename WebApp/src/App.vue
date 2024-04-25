<script setup lang="ts">
import { ref } from 'vue'
import MqttViaWebsocketWorker from './workers/MqttViaWebsocket.worker.ts?worker'
import MqttViaSignalRWorker from './workers/MqttViaSignalR.worker.ts?worker'
import StreamViaSignalRWorker from './workers/StreamViaSignalR.worker.ts?worker'

const mqttViaWebsocketString = ref('init')
let mqttViaWebsocketCount = 0
const mqttViaWebsocketLastSecondCount = ref(0)
const mqttViaWebsocketWorker = new MqttViaWebsocketWorker()
mqttViaWebsocketWorker.addEventListener('message', (event) => {
  mqttViaWebsocketString.value = `vhl: ${(event.data.vhl as number).toString().padStart(4, '0')} | tick: ${event.data.tick}`
  mqttViaWebsocketCount++
})

const mqttViaSignalRString = ref('init')
let mqttViaSignalRCount = 0
const mqttViaSignalRLastSecondCount = ref(0)
const mqttViaSignalRWorker = new MqttViaSignalRWorker()
mqttViaSignalRWorker.addEventListener('message', (event) => {
  mqttViaSignalRString.value = `vhl: ${(event.data.vhl as number).toString().padStart(4, '0')} | tick: ${event.data.tick}`
  mqttViaSignalRCount++
})
const streamViaSignalRString = ref('init')
let streamViaSignalRCount = 0
const streamViaSignalRLastSecondCount = ref(0)
const streamViaSignalRWorker = new StreamViaSignalRWorker()
streamViaSignalRWorker.addEventListener('message', (event) => {
  streamViaSignalRString.value = `vhl: ${(event.data.vhl as number).toString().padStart(4, '0')} | tick: ${event.data.tick}`
  streamViaSignalRCount++
})

setInterval(() => {
  mqttViaWebsocketLastSecondCount.value = mqttViaWebsocketCount
  mqttViaWebsocketCount = 0
  mqttViaSignalRLastSecondCount.value = mqttViaSignalRCount
  mqttViaSignalRCount = 0
  streamViaSignalRLastSecondCount.value = streamViaSignalRCount
  streamViaSignalRCount = 0
}, 1000)

</script>

<template>
  <h1>rabbitmq mqtt perf page</h1>

  <ul>
    <li>
      <span class="from">MQTT via Websocket:</span>
      <span class="data">{{ mqttViaWebsocketString }} </span>
      <span class="count">{{ mqttViaWebsocketLastSecondCount }}</span>
      <div>mqtt->mqtt-ws</div>
    </li>
    <li>
      <span class="from">MQTT via SignalR: </span>
      <span class="data">{{ mqttViaSignalRString }} </span>
      <span class="count">{{ mqttViaSignalRLastSecondCount }}</span>
      <div>mqtt->signalR </div>
    </li>
    <li>
      <span class="from">Stream via SignalR:</span>
      <span class="data">{{ streamViaSignalRString }}</span>
      <span class="count">{{ streamViaSignalRLastSecondCount }}</span>
      <div>mqtt->stream(exchange)->signalR</div>
    </li>
  </ul>
</template>

<style scoped>
ul {
  width: 100%;
  text-align: left;
}

.from {
  display: inline-block;
  width: 200px;
  font-weight: bolder;
}

.data {
  display: inline-block;
  width: 200px;
  color: brown;
  font-size: larger;
}

.count {
  display: inline-block;
  width: 100px;
  color: tomato;
}
</style>
