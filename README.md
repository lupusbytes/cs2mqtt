<p align="center">
    <a href="https://github.com/lupusbytes/cs2mqtt/actions/workflows/build.yml"><img src="https://github.com/lupusbytes/cs2mqtt/actions/workflows/build.yml/badge.svg"></a>
    <a href="https://github.com/lupusbytes/cs2mqtt/actions/workflows/docker.yml"><img src="https://github.com/lupusbytes/cs2mqtt/actions/workflows/docker.yml/badge.svg"></a>
    <a href="https://codecov.io/gh/lupusbytes/cs2mqtt"><img src="https://codecov.io/gh/lupusbytes/cs2mqtt/graph/badge.svg?token=FJYRCDUDRH"></a>
    <a href="https://sonarcloud.io/summary/new_code?id=lupusbytes_cs2mqtt"><img src="https://sonarcloud.io/api/project_badges/measure?project=lupusbytes_cs2mqtt&metric=security_rating"></a>
    <a href="https://sonarcloud.io/summary/new_code?id=lupusbytes_cs2mqtt"><img src="https://sonarcloud.io/api/project_badges/measure?project=lupusbytes_cs2mqtt&metric=reliability_rating"></a>
    <a href="https://sonarcloud.io/summary/new_code?id=lupusbytes_cs2mqtt"><img src="https://sonarcloud.io/api/project_badges/measure?project=lupusbytes_cs2mqtt&metric=sqale_rating"></a>
</p>

# cs2mqtt
## Introduction
This project allows you to integrate your [Counter-Strike 2](https://store.steampowered.com/app/730/CounterStrike_2/) game with [Home Assistant](https://www.home-assistant.io/) by exposing it as an [MQTT device](https://www.home-assistant.io/integrations/mqtt/).
It allows you to automate your smart home based on in-game events and information, enhancing your overall gaming experience.

## About
cs2mqtt receives its data from the built-in [Counter-Strike Game State Integration](https://developer.valvesoftware.com/wiki/Counter-Strike:_Global_Offensive_Game_State_Integration) engine. This integration is created by Valve and is safe to use and will not trigger anti-cheat systems such as VAC or FACEIT.

cs2mqtt is cross-platform and can run locally on your gaming machine or any server device.
