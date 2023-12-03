﻿using FluentCMS.Entities;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace FluentCMS;

public class JsonContentConverter() : JsonConverter<Content>
{
    private readonly static JsonConverter<string> stringConv =
        (JsonConverter<string>)JsonSerializerOptions.Default.GetConverter(typeof(string));

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(Content);
    }

    public override Content? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        var content = new Content();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return content;

            // Get the key.
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string? key = reader.GetString() ?? throw new JsonException();

            // Get the value.
            reader.Read();

            switch (key.ToLower())
            {
                case "id":
                    content.Id = reader.GetGuid();
                    continue;

                case "siteid":
                    content.SiteId = reader.GetGuid();
                    continue;

                case "type":
                    content.Type = reader.GetString() ?? string.Empty;
                    continue;

                case "createdby":
                    content.CreatedBy = reader.GetString() ?? string.Empty;
                    continue;

                case "lastupdatedby":
                    content.LastUpdatedBy = reader.GetString() ?? string.Empty;
                    continue;

                case "createdat":
                    content.CreatedAt = reader.GetDateTime();
                    continue;

                case "lastupdatedat":
                    content.LastUpdatedAt = reader.GetDateTime();
                    continue;
                    
                default:
                    break;
            }

            // Add to dictionary.
            content.Add(key, ExtractValue(ref reader, options));
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Content content, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Id");
        writer.WriteStringValue(content.Id);

        writer.WritePropertyName("SiteId");
        writer.WriteStringValue(content.SiteId);

        writer.WritePropertyName("Type");
        writer.WriteStringValue(content.Type);

        writer.WritePropertyName("CreatedAt");
        writer.WriteStringValue(content.CreatedAt);

        writer.WritePropertyName("CreatedBy");
        writer.WriteStringValue(content.CreatedBy);

        writer.WritePropertyName("LastUpdatedAt");
        writer.WriteStringValue(content.LastUpdatedAt);

        writer.WritePropertyName("LastUpdatedBy");
        writer.WriteStringValue(content.LastUpdatedBy);


        foreach ((string key, object value) in content)
        {
            var propertyName = key.ToString();
            writer.WritePropertyName(propertyName);
                //(options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);
            
            stringConv.Write(writer, value.ToString(), options);
        }

        writer.WriteEndObject();
    }

    private object? ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (reader.TryGetDateTime(out var date))
                {
                    return date;
                }
                return reader.GetString();
            case JsonTokenType.False:
                return false;
            case JsonTokenType.True:
                return true;
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var result))
                {
                    return result;
                }
                return reader.GetDecimal();
            case JsonTokenType.StartObject:
                return Read(ref reader, null!, options);
            case JsonTokenType.StartArray:
                var list = new List<object?>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }
                return list;
            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }

}
