using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;
using LupusBytes.CS2.GameStateIntegration.Events;
using Microsoft.Extensions.Hosting;

namespace LupusBytes.CS2.GameStateIntegration.Mqtt.HomeAssistant;

public sealed class HomeAssistantDevicePublisher(
    IGameStateService gameStateService,
    IMqttClient mqttClient) :
    BackgroundService,
    IObserver<BaseEvent>
{
    private readonly Channel<BaseEvent> channel = Channel.CreateBounded<BaseEvent>(new BoundedChannelOptions(1000)
    {
        SingleWriter = false,
        SingleReader = true,
        FullMode = BoundedChannelFullMode.DropOldest,
    });

    private readonly ConcurrentDictionary<string, DeviceState> devices = [];

    public void OnNext(BaseEvent value) => channel.Writer.TryWrite(value);

    public void OnCompleted()
    {
    }

    public void OnError(Exception error) => throw error;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var playerSubscription = gameStateService.Subscribe(this as IObserver<PlayerEvent>);
        using var playerStateSubscription = gameStateService.Subscribe(this as IObserver<PlayerStateEvent>);
        using var roundSubscription = gameStateService.Subscribe(this as IObserver<RoundEvent>);
        using var mapSubscription = gameStateService.Subscribe(this as IObserver<MapEvent>);

        await ProcessChannelAsync(stoppingToken);
    }

    private async Task ProcessChannelAsync(CancellationToken cancellationToken)
    {
        while (await channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (channel.Reader.TryRead(out var @event))
            {
                var deviceId = @event.SteamId.ToString();
                var deviceState = devices.GetOrAdd(
                    deviceId,
                    key => new DeviceState(
                        new Device(
                            Name: "CS2",
                            Id: key,
                            Manufacturer: "lupusbytes",
                            Model: "cs2mqtt",
                            SoftwareVersion: "0.0.1-beta")));

                deviceState = @event switch
                {
                    PlayerEvent p => await UpdatePlayerSensorsAsync(deviceState, p.Player is not null, cancellationToken),
                    PlayerStateEvent ps => await UpdatePlayerStateSensorsAsync(deviceState, ps.PlayerState is not null, cancellationToken),
                    MapEvent m => await UpdateMapSensorsAsync(deviceState, m.Map is not null, cancellationToken),
                    RoundEvent r => await UpdateRoundSensorsAsync(deviceState, r.Round is not null, cancellationToken),
                    _ => throw new SwitchExpressionException(),
                };

                devices[deviceId] = deviceState;
            }
        }
    }

    private Task<DeviceState> UpdatePlayerSensorsAsync(
        DeviceState deviceState,
        bool eventHadData,
        CancellationToken cancellationToken) =>
        UpdateSensorsAsync(
            deviceState,
            new PlayerSensors(deviceState.Device),
            eventHadData,
            d => d.PlayerSensorsAnnounced,
            d => d with { PlayerSensorsAnnounced = true },
            d => d.PlayerAvailability,
            d => d with { PlayerAvailability = eventHadData },
            cancellationToken);

    private Task<DeviceState> UpdatePlayerStateSensorsAsync(
        DeviceState deviceState,
        bool eventHadData,
        CancellationToken cancellationToken) =>
        UpdateSensorsAsync(
            deviceState,
            new PlayerStateSensors(deviceState.Device),
            eventHadData,
            d => d.PlayerStateSensorsAnnounced,
            d => d with { PlayerStateSensorsAnnounced = true },
            d => d.PlayerStateAvailability,
            d => d with { PlayerStateAvailability = eventHadData },
            cancellationToken);

    private Task<DeviceState> UpdateMapSensorsAsync(
        DeviceState deviceState,
        bool eventHadData,
        CancellationToken cancellationToken) =>
        UpdateSensorsAsync(
            deviceState,
            new MapSensors(deviceState.Device),
            eventHadData,
            d => d.MapSensorsAnnounced,
            d => d with { MapSensorsAnnounced = true },
            d => d.MapAvailability,
            d => d with { MapAvailability = eventHadData },
            cancellationToken);

    private Task<DeviceState> UpdateRoundSensorsAsync(
        DeviceState deviceState,
        bool eventHadData,
        CancellationToken cancellationToken) =>
        UpdateSensorsAsync(
            deviceState,
            new RoundSensors(deviceState.Device),
            eventHadData,
            d => d.RoundSensorsAnnounced,
            d => d with { RoundSensorsAnnounced = true },
            d => d.RoundAvailability,
            d => d with { RoundAvailability = eventHadData },
            cancellationToken);

    private async Task<DeviceState> UpdateSensorsAsync(
        DeviceState deviceState,
        IDeviceSensors sensors,
        bool eventHadData,
        Func<DeviceState, bool> getSensorsAnnounced,
        Func<DeviceState, DeviceState> setSensorsAnnounced,
        Func<DeviceState, bool> getAvailability,
        Func<DeviceState, DeviceState> setAvailability,
        CancellationToken cancellationToken)
    {
        if (eventHadData && !getSensorsAnnounced(deviceState))
        {
            await SendDiscoveryPayloadsAsync(sensors, cancellationToken);
            deviceState = setSensorsAnnounced(deviceState);
        }

        if (eventHadData == getAvailability(deviceState))
        {
            return deviceState;
        }

        await SetSensorAvailabilityAsync(sensors, eventHadData, cancellationToken);
        deviceState = setAvailability(deviceState);
        return deviceState;
    }

    private async Task SendDiscoveryPayloadsAsync(
        IDeviceSensors deviceSensors,
        CancellationToken cancellationToken)
    {
        foreach (var discoveryPayload in deviceSensors.DiscoveryPayloads)
        {
            var message = new MqttMessage
            {
                Topic = $"homeassistant/sensor/{discoveryPayload.UniqueId}/config",
                Payload = JsonSerializer.Serialize(discoveryPayload),
            };

            await mqttClient.PublishAsync(message, cancellationToken);
        }
    }

    private async Task SetSensorAvailabilityAsync(
        IDeviceSensors deviceSensors,
        bool available,
        CancellationToken cancellationToken)
    {
        var availabilityTopics = deviceSensors
            .DiscoveryPayloads
            .Select(x => x.Availability)
            .Select(x => x.Topic)
            .Distinct(StringComparer.Ordinal);

        foreach (var topic in availabilityTopics)
        {
            var message = new MqttMessage
            {
                Topic = topic,
                Payload = available ? "online" : "offline",
            };

            await mqttClient.PublishAsync(message, cancellationToken);
        }
    }

    private sealed record DeviceState(
        Device Device,
        bool PlayerSensorsAnnounced = false,
        bool PlayerAvailability = false,
        bool PlayerStateSensorsAnnounced = false,
        bool PlayerStateAvailability = false,
        bool MapSensorsAnnounced = false,
        bool MapAvailability = false,
        bool RoundSensorsAnnounced = false,
        bool RoundAvailability = false);
}