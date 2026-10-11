using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductService.Api.Queries.Dtos;

namespace BlazorWasmClient.Components.Products;

public sealed class ProductQuestionConverter : JsonConverter<QuestionDto>
{
    public override bool CanWrite => false;

    public override QuestionDto ReadJson(
        JsonReader reader,
        Type objectType,
        QuestionDto existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            throw new JsonSerializationException("A product question cannot be null.");

        var questionJson = JObject.Load(reader);
        var questionType = questionJson.GetValue("questionType", StringComparison.OrdinalIgnoreCase);

        if (questionType?.Type != JTokenType.String)
            throw new JsonSerializationException("A product question must have a string questionType.");

        QuestionDto question = questionType.Value<string>() switch
        {
            "Choice" => new ChoiceQuestionDto(),
            "Numeric" => new NumericQuestionDto(),
            var value => throw new JsonSerializationException($"Unsupported product questionType '{value}'.")
        };

        using var questionReader = questionJson.CreateReader();
        serializer.Populate(questionReader, question);
        return question;
    }

    public override void WriteJson(JsonWriter writer, QuestionDto value, JsonSerializer serializer)
        => throw new NotSupportedException("This converter is only used to read product questions.");
}
