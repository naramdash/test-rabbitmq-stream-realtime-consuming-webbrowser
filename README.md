# Performance test for sending mqtt message to web browser using RabbitMQ

https://github.com/naramdash/test-rabbitmq-stream-realtime-consuming-webbrowser/assets/24370799/d6d337a8-4af3-43f1-b309-b23083ab0d54

# Before test

## 1. enable rabbitmq plugins

- rabbitmq_mqtt
- rabbitmq_web_mqtt
- rabbitmq_stream
- rabbitmq_management

## 2. Enable feature flags

`sudo rabbitmqctl enable_feature_flag all`

## 3. make stream

- type `stream`
- name `iot-sensor-v34-status`
- binding
  - from exchange: `amq.topic`
  - routing key: `iot.sensor.v34.*.status`

## Extra.

- Set mqtt default credential if each server connected by LAN
- If you want adjust firing msgs, check
  - `ServerMqttPublisher` / `MqttPublishService.fs` / `vhlCount`
  - `ServerMqttPublisher` / `MqttPublishService.fs` / `interval`
  - `ServerMqttPublisher` / `JsonSample.fs` / `getJsonSample`
  - `ServerMqttToSignalR` / `MessageType.fs` / `Message`
  - `ServerStreamToSignalR` / `MessageType.fs` / `Message`

# Test

1. `./Servers > dotnet build`
1. `./Servers > dotnet run --project ./ServerMqttPublisher`
1. `./Servers > dotnet run --project ./ServerMqttToSignalR`
1. `./Servers > dotnet run --project ./ServerStreamToSignalR`
1. `./WebApp > npm install`
1. `./WebApp > npm run dev`
1. `open web browser`
