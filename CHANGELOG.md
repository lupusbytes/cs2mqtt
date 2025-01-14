# Changelog

## [1.5.2](https://github.com/lupusbytes/cs2mqtt/compare/v1.5.1...v1.5.2) (2025-01-14)


### Upgrades

* **deps:** update dependency polly to 8.5.1 ([55e1eb8](https://github.com/lupusbytes/cs2mqtt/commit/55e1eb8b5a9ad27772dee7b04729d36c75400095))
* **deps:** update dotnet monorepo to 9.0.1 ([57b9794](https://github.com/lupusbytes/cs2mqtt/commit/57b9794fad6c311774973c3197cf5f35fb7c478f))

## [1.5.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.5.0...v1.5.1) (2025-01-11)


### Bug fixes

* **homeassistant:** fix bridge device being connected via unnamed device ([aeff30a](https://github.com/lupusbytes/cs2mqtt/commit/aeff30a74f3298984c6d21a705be86f91b89b6e8))

## [1.5.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.4.1...v1.5.0) (2025-01-08)


### New features

* upgrade to dotnet 9 ([219ff68](https://github.com/lupusbytes/cs2mqtt/commit/219ff6890a0083745bf60a78980031ba8d6447c4))

## [1.4.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.4.0...v1.4.1) (2025-01-05)


### Upgrades

* **deps:** update dependency mqttnet to 5.0.1.1416 ([56223ca](https://github.com/lupusbytes/cs2mqtt/commit/56223ca45efb1493029cc0a1bff06e9d56a67045))

## [1.4.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.3.0...v1.4.0) (2025-01-05)


### New features

* **mqtt:** add protocol version as an optional option ([85f2895](https://github.com/lupusbytes/cs2mqtt/commit/85f28956e5b860b3bbf33d455edde13fdf2c13e4))


### Upgrades

* **deps:** update dependency mqttnet from v4 to v5 ([2953184](https://github.com/lupusbytes/cs2mqtt/commit/29531841e2dc6aba0e1953749fedc4b9f930484b))

## [1.3.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.2.1...v1.3.0) (2024-10-27)


### Features

* **homeassistant:** append textual steamid (steam_x:y:z) to the mqtt device name ([9193d1a](https://github.com/lupusbytes/cs2mqtt/commit/9193d1aa51a04dee3bf5ee67d5263e893de7ced6))
* **homeassistant:** publish cs2mqtt as a bridge device; use it as via_device for game state devices ([98148ec](https://github.com/lupusbytes/cs2mqtt/commit/98148ece21312f85226f4277593c47d34e1b338f))

## [1.2.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.2.0...v1.2.1) (2024-10-13)


### Bug Fixes

* **homeassistant:** show "inactive" for bomb when value is null instead of "unknown" ([7292ef1](https://github.com/lupusbytes/cs2mqtt/commit/7292ef174bd7cc80cc5cf6befc1b39a819386e1e))

## [1.2.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.1.2...v1.2.0) (2024-10-02)


### Features

* **mqtt:** publish offline message to all availability topics on graceful shutdown ([30db192](https://github.com/lupusbytes/cs2mqtt/commit/30db19297262c29ef508a36187298950faa5b304))

## [1.1.2](https://github.com/lupusbytes/cs2mqtt/compare/v1.1.1...v1.1.2) (2024-09-17)


### Bug Fixes

* **homeassistant:** use `availability_mode: all` for mqtt entities ([32e8991](https://github.com/lupusbytes/cs2mqtt/commit/32e8991b19d8d078e5e817bc8d62b4ea2139c8b8))

## [1.1.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.1.0...v1.1.1) (2024-09-15)


### Bug Fixes

* **mqtt:** ensure no broker reconnect is attempted on graceful shutdown ([4e0def7](https://github.com/lupusbytes/cs2mqtt/commit/4e0def7c567d46362f57c0cde5aa232ae80b8751))
* **mqtt:** publish offline message to system availability topic on graceful shutdown ([5248c52](https://github.com/lupusbytes/cs2mqtt/commit/5248c52c22ba30afdc6d3e711123e28075d6359a))

## [1.1.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.0.0...v1.1.0) (2024-08-11)


### Features

* **homeassistant:** use assembly version for mqtt device sw version ([9c64973](https://github.com/lupusbytes/cs2mqtt/commit/9c64973a1b7a373e1d968dc1e2bab6547c122a9a))

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
