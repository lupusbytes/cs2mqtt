using System.Text.Json.Serialization;

namespace LupusBytes.CS2.GameStateIntegration.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Mode
{
    /// <summary>
    /// Like Competitive but with fewer rounds, shorter freezetime per round,
    /// no friendly fire, no team collision, free armor and free defuse kit/cutters.
    /// </summary>
    Casual,

    /// <summary>
    /// The classic game mode, usually made for 5v5.
    /// Best of 24 rounds, teams are switched at halftime, friendly fire is on.
    /// Round-ending objectives are elimination, bomb explosion, bomb defusal, hostage rescue and timeout
    /// </summary>
    Competitive,

    /// <summary>
    /// Wingman.
    /// Like Competitive, but adjusted for 2v2 and for a smaller map or a map section.
    /// Best of 16 rounds. Also, each round is shorter.
    /// </summary>
    ScrimComp2v2,

    /// <summary>
    /// Weapons Expert.
    /// Like Competitive but every player can buy each weapon at most once per match.
    /// </summary>
    ScrimComp5v5,

    /// <summary>
    /// Arms Race.
    /// The game is one perpetual round where killed players respawn at the default spawns.
    /// A weapon progression (a number of guns and their order) is defined where players win by making a specified
    /// number of kills with each of these weapons.
    /// </summary>
    GunGameProgressive,

    /// <summary>
    /// Demolition.
    /// A mixture of Casual with Armsrace.
    /// Best of 20 rounds.
    /// Each player is given a fixed weapon for each round, depending on their individual progress.
    /// Each player can progress one gun per round by making at least one kill.
    /// </summary>
    GunGameTRBomb,

    /// <summary>
    /// Deathmatch mode.
    /// </summary>
    Deathmatch,

    /// <summary>
    /// Used on CS:GO's Training Course.
    /// </summary>
    Training,

    /// <summary> .
    /// This is an "empty" game mode allowing mapmakers to implement any map-specific custom game mode.
    /// </summary>
    Custom,

    /// <summary>
    /// Two human players must defend a bombsite as CT or hostages as T against rushing bots.
    /// </summary>
    Cooperative,

    /// <summary>
    /// Usually a custom mission for two human players on a custom map that is designed primarily for this mode.
    /// </summary>
    CoopMission,

    /// <summary>
    /// War Games.
    /// </summary>
    Skirmish,

    /// <summary>
    /// Danger Zone.
    /// A Battle Royale mode for big maps where players win by being the last man (or team) standing.
    /// Maps must be designed for this mode to work as intended.
    /// </summary>
    Survival,

    /// <summary>
    /// Seems not implemented yet. Appears in the file `csgo/pak01_dir.vpk/gamemodes_tools.txt`
    /// </summary>
    Workshop,

    /// <summary>
    /// Retakes mode.
    /// Each round, 3 Terrorists spawn on a bomb site with a bomb being planted
    /// and 4 CTs spawn at fixed locations around it or on the other bomb spot.
    /// Each player can choose a loadout card at round start.
    /// </summary>
    Retakes,

#pragma warning disable CA1707
    /// <summary>
    /// Training day.
    /// Like Competitive with bots but automatically assigning humans to Terrorists with game instructions turned on.
    /// </summary>
    /// <remarks>
    /// This mode is actually called "new_user_training", but we convert it to screaming snake case.
    /// It is the only <see cref="Mode"/> to contain underscores.
    /// </remarks>
    NEW_USER_TRAINING,
#pragma warning restore CA1707
}