using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// <see cref="GameVersion.ORAS"/> encounter area
/// </summary>
public sealed record EncounterArea6AO : IEncounterArea<EncounterSlot6AO>, IAreaLocation
{
    public EncounterSlot6AO[] Slots { get; }
    public GameVersion Version { get; }

    public readonly ushort Location;
    public readonly SlotType Type;

    public bool IsMatchLocation(int location) => Location == location;

    public static EncounterArea6AO[] GetAreas(BinLinkerAccessor input, GameVersion game)
    {
        var result = new EncounterArea6AO[input.Length];
        for (int i = 0; i < result.Length; i++)
            result[i] = new EncounterArea6AO(input[i], game);
        return result;
    }

    private EncounterArea6AO(ReadOnlySpan<byte> data, GameVersion game)
    {
        Location = ReadUInt16LittleEndian(data);
        Type = (SlotType)data[2];
        Version = game;

        Slots = ReadSlots(data);
    }

    private EncounterSlot6AO[] ReadSlots(ReadOnlySpan<byte> data)
    {
        const int size = 4;
        int count = (data.Length - 4) / size;
        var slots = new EncounterSlot6AO[count];
        for (int i = 0; i < slots.Length; i++)
        {
            int offset = 4 + (size * i);
            var entry = data.Slice(offset, size);
            slots[i] = ReadSlot(entry);
        }

        return slots;
    }

    private EncounterSlot6AO ReadSlot(ReadOnlySpan<byte> entry)
    {
        ushort species = ReadUInt16LittleEndian(entry);
        byte form = (byte)(species >> 11);
        species &= 0x3FF;
        byte min = entry[2];
        byte max = entry[3];
        return new EncounterSlot6AO(this, species, form, min, max);
    }
}
