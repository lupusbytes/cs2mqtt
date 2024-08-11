## 1.0.0 (2024-08-12)

### Features

* Bridge events from Counter-Strike 2 Game State Integration to any MQTT broker
* Separate events from multiple providers into their own MQTT topics
* Keep track of the latest game state and emit the same data only once, using the MQTT retain flag
* Allow providers to limit and customize the data included in their payloads
* Maintain a system-wide availability topic
* Maintain separate availability topics for each type of game state component, for each provider
* Automatically detect when providers close their game/stop sending data and emit availability status for them
* Utilize the HomeAssistant MQTT device discovery protocol to automatically create a `cs2mqtt` device for each provider, with all data exposed as sensors
* Expose data on REST API endpoints
* Optionally use pre-shared key (token) auth from CS2 Game State Integration on HTTP ingestion endpoint
* Optionally use TLS, custom client ID and credentials for MQTT broker