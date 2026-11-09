using System;
using System.Buffers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LinkDotNet.Blog.Domain;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features;

public sealed class PersistedStateSerializer<T> : PersistentComponentStateSerializer<T>
{
    public override void Persist(T value, IBufferWriter<byte> writer)
    {
        using var json = new Utf8JsonWriter(writer);
        JsonSerializer.Serialize(json, value, PersistedStateJson.Options);
    }

    public override T Restore(ReadOnlySequence<byte> data)
    {
        var reader = new Utf8JsonReader(data);
        return JsonSerializer.Deserialize<T>(ref reader, PersistedStateJson.Options)!;
    }
}

internal static class PersistedStateJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { AllowNonPublicConstructionOfEntities } },
        Converters = { new ProficiencyLevelConverter() },
    };

    private static void AllowNonPublicConstructionOfEntities(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object || !typeof(Entity).IsAssignableFrom(typeInfo.Type))
        {
            return;
        }

        typeInfo.CreateObject ??= () => RuntimeHelpers.GetUninitializedObject(typeInfo.Type);
        foreach (var property in typeInfo.Properties)
        {
            if (property.Set is null && property.AttributeProvider is PropertyInfo info && info.GetSetMethod(nonPublic: true) is { } setter)
            {
                property.Set = (target, value) => setter.Invoke(target, [value]);
            }
        }
    }

    private sealed class ProficiencyLevelConverter : JsonConverter<ProficiencyLevel>
    {
        public override ProficiencyLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => ProficiencyLevel.Create(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, ProficiencyLevel value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Key);
    }
}
