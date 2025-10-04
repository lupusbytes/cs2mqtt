# Changelog

## [1.9.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.9.0...v1.9.1) (2025-10-04)


### Bug fixes

* **homeassistant:** gracefully handle empty payloads in value_templates ([cd8298d](https://github.com/lupusbytes/cs2mqtt/commit/cd8298d83be97a4dd848595521cc01d2d63e6f23))

## [1.9.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.8.1...v1.9.0) (2025-09-30)


### New features

* add option to ignore data from spectated players. it is now enabled by default and must be disabled to revert to previous behavior. ([8573417](https://github.com/lupusbytes/cs2mqtt/commit/8573417e87cb714b7c93e93bbcd0cf40146eca2f))
* **ha-addon:** add cs2mqtt icon and logo ([e1c1134](https://github.com/lupusbytes/cs2mqtt/commit/e1c11346c35c9a04f409b26c8c7b74bc2b3c62ab))
* **ha-addon:** expose more cs2mqtt options on configuration page and tidy up ([41ddace](https://github.com/lupusbytes/cs2mqtt/commit/41ddacef43050e27c0b4c8115af791c06067fe64))
* **ha-addon:** use /alive endpoint for watchdog health checks ([e886dce](https://github.com/lupusbytes/cs2mqtt/commit/e886dce79bac237621016197926a4c0c0835bd19))
* **logging:** add timestamps to log messages ([59b214a](https://github.com/lupusbytes/cs2mqtt/commit/59b214a552eccd8186b91d25497a19251d925a47))
* **logging:** log cs2 gsi api requests at debug level ([ad118ee](https://github.com/lupusbytes/cs2mqtt/commit/ad118eedb5dfcc6b2d1c8152c8eb9df428d1afd9))

## [1.8.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.8.0...v1.8.1) (2025-09-26)


### Bug fixes

* **mqtt:** fix availability bug preventing sensor from transitioning offline -&gt; online ([e2a50c7](https://github.com/lupusbytes/cs2mqtt/commit/e2a50c7237233c2a7b92ba81407a06091c288cf4))


### Performance improvements

* in the documentation for `gamestate_integration_cs2mqtt.cfg`, it originally had `"heartbeat" "60.0"`, but it turns out this value also determines how long cs2 waits before sending player game state data when joining a deathmatch game or when switching from free cam to spectating a player in a casual game and more. the new recommended value is `"heartbeat" "5.0"`. please update your existing configs as you see fit. ([6deb9d7](https://github.com/lupusbytes/cs2mqtt/commit/6deb9d7e56695e81e170855650d2b3ae3765e93d))

## [1.8.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.7.0...v1.8.0) (2025-09-25)


### New features

* **homeassistant:** add cs2 game device connectivity sensor ([8c3b03c](https://github.com/lupusbytes/cs2mqtt/commit/8c3b03ce3c6683340838747ec4ea8bfc4c31c13a))
* **homeassistant:** convert flashed sensor to boolean value ([2ba6cb0](https://github.com/lupusbytes/cs2mqtt/commit/2ba6cb0210fa06c8e90c9936a7427d7b9238fd76))
* **homeassistant:** convert smoked and burning sensor measurements to percentage ([305a0fd](https://github.com/lupusbytes/cs2mqtt/commit/305a0fd1dfd9523c74f0b4e895521f9c5040c67d))
* **homeassistant:** use state_class: 'measurement' for numerical sensor values ([c3b1808](https://github.com/lupusbytes/cs2mqtt/commit/c3b18082a5080891236f5e59106310cfe45e050f))


### Bug fixes

* **mqtt:** correctly log connection retry delay when wait is over 1 minute ([1f30728](https://github.com/lupusbytes/cs2mqtt/commit/1f307280ad909314af5c7283c4adea6999bcf309))
* **mqtt:** ensure cs2mqtt shuts down after max reconnect attempts ([2d8c139](https://github.com/lupusbytes/cs2mqtt/commit/2d8c139e86c60c717414eb5be68e2805641726d7))

## [1.7.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.9...v1.7.0) (2025-09-19)


### New features

* **homeassistant:** add unit of measurement to various sensors ([f520b83](https://github.com/lupusbytes/cs2mqtt/commit/f520b8319fe9f099a4f1292b1512894046ba6cb5))
* **homeassistant:** enable installing cs2mqtt as an add-on via repository ([06e5f1f](https://github.com/lupusbytes/cs2mqtt/commit/06e5f1f50db13d611fdf440406952a492fbc4f4e))


### Bug fixes

* **mqtt:** throw exception when authentication fails ([d0177b6](https://github.com/lupusbytes/cs2mqtt/commit/d0177b6600981a9ddafeced81a2f77aaefde114a))

## [1.6.9](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.8...v1.6.9) (2025-09-16)


### Upgrades

* **deps:** update dependency polly to 8.6.3 ([262314a](https://github.com/lupusbytes/cs2mqtt/commit/262314a3195d78c529b066dd1ec1a41f3fb6a429))
* **deps:** update dotnet monorepo to 9.0.9 ([3033e9e](https://github.com/lupusbytes/cs2mqtt/commit/3033e9e353f5418fe4699bc572bb013fc465de3a))

## [1.6.8](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.7...v1.6.8) (2025-08-05)


### Upgrades

* **deps:** update dotnet monorepo to 9.0.8 ([c949a9c](https://github.com/lupusbytes/cs2mqtt/commit/c949a9cb53b09ac44fc42c3f2fc2048aeb3de4c7))

## [1.6.7](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.6...v1.6.7) (2025-07-15)


### Upgrades

* **deps:** update dependency polly to 8.6.2 ([81fbbd7](https://github.com/lupusbytes/cs2mqtt/commit/81fbbd76090041a9fa76dfe1c009e6278491cfbf))
* **deps:** update dotnet monorepo to 9.0.7 ([9e7ba36](https://github.com/lupusbytes/cs2mqtt/commit/9e7ba363c2e87c90919f4914897ae8bb8dc0e460))

## [1.6.6](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.5...v1.6.6) (2025-06-12)


### Upgrades

* **deps:** update dependency polly to 8.6.0 ([687880b](https://github.com/lupusbytes/cs2mqtt/commit/687880b0ad2aa6628df2aa8c1a71711aa0d94612))
* **deps:** update dotnet monorepo to 9.0.6 ([33975e4](https://github.com/lupusbytes/cs2mqtt/commit/33975e48611ebb89176f64c1f609b996b751030b))

## [1.6.5](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.4...v1.6.5) (2025-05-15)


### Upgrades

* **deps:** update dotnet monorepo to 9.0.5 ([a86f4b3](https://github.com/lupusbytes/cs2mqtt/commit/a86f4b3ac00760bac592efbb80b66abfb9e71373))

## [1.6.4](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.3...v1.6.4) (2025-04-24)


### Upgrades

* **deps:** update dotnet monorepo to 9.0.4 ([2105f4e](https://github.com/lupusbytes/cs2mqtt/commit/2105f4e00ffb1fc7951702df68ea15a44311e9e9))

## [1.6.3](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.2...v1.6.3) (2025-03-11)


### Upgrades

* **deps:** update dotnet monorepo to 9.0.3 ([7f32ef2](https://github.com/lupusbytes/cs2mqtt/commit/7f32ef21f84553abd841b766193a41c6133e873a))

## [1.6.2](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.1...v1.6.2) (2025-02-11)


### Upgrades

* **deps:** update dotnet packages to 9.0.2 ([a74f2f4](https://github.com/lupusbytes/cs2mqtt/commit/a74f2f455e690e6b40750e7319d4753efc4d173b))

## [1.6.1](https://github.com/lupusbytes/cs2mqtt/compare/v1.6.0...v1.6.1) (2025-02-06)


### Upgrades

* **deps:** update dependency polly to 8.5.2 ([71f125c](https://github.com/lupusbytes/cs2mqtt/commit/71f125c32e323f1e24e5db8877123a4f86b357a0))

## [1.6.0](https://github.com/lupusbytes/cs2mqtt/compare/v1.5.2...v1.6.0) (2025-01-20)


### New features

* **api:** add healthcheck endpoints /health and /alive ([ad9c6cd](https://github.com/lupusbytes/cs2mqtt/commit/ad9c6cd1b4317d78344278ca38840607c7b4e590))


### Bug fixes

* remove duplicate binding of gamestate options ([308821d](https://github.com/lupusbytes/cs2mqtt/commit/308821d7c7473ecdc5ee369c8fdf24d4e3140044))

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
