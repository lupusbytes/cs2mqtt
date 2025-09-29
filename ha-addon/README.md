### Connecting **Counter-Strike 2** to **cs2mqtt**

To make **Counter-Strike 2** send game state data to **cs2mqtt**, you need to create a config file within the game's directory.

If **Steam** and **Counter-Strike 2** are installed with default options, the path where you need to create your config is:

```
C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\game\csgo\cfg
```

Once you have located the correct path, create a new file there called **gamestate_integration_cs2mqtt.cfg** with the following content, replacing the URI part with your Home Assistant server IP:

```
"cs2mqtt"
{
 "uri" "http://YOUR-HOMEASSISTANT-SERVER-IP:5000"
 "timeout" "5.0"
 "buffer"  "0.1"
 "throttle" "0.1"
 "heartbeat" "5.0"
 "data"
 {
   "provider"            "1"
   "map"                 "1"
   "round"               "1"
   "player_id"           "1"
   "player_state"        "1"
 }
}
```

💡 Tip  
In Windows File Explorer, it can be tricky to create a new file with a specific file extension (like `.cfg`).  
A simple workaround is to copy one of the many pre-existing `.cfg` files in the folder, rename it, and then replace its contents using Notepad.


The next time you launch **Counter-Strike 2**, **cs2mqtt** will use the [MQTT discovery protocol](https://www.home-assistant.io/integrations/mqtt/#mqtt-discovery) to make [Home Assistant](https://www.home-assistant.io/) create an MQTT device named `CS2 STEAM_{X}:{Y}:{Z}`, matching your SteamID. On the very first game launch, the device will not have many sensors, but after joining or creating a game server, all sensors will automatically be discovered.

### Limiting what game state data is sent

It is possible to remove any of the following entries from the data object in **gamestate_integration_cs2mqtt.cfg**:

-   `map`
-   `round`
-   `player_id`
-   `player_state`

This will stop **Counter-Strike 2** from sending data about the removed topics.  
The only required entry is `provider`.
