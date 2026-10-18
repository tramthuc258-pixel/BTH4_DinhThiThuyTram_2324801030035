using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PricingService.Api.Commands.Dto;

namespace PricingService.Api.Converters;

internal class QuestionAnswerConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => typeof(QuestionAnswer).IsAssignableFrom(objectType);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;
        return Create(JObject.Load(reader));
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is QuestionAnswer questionAnswer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("questionCode");
            serializer.Serialize(writer, questionAnswer.QuestionCode);
            writer.WritePropertyName("questionType");
            serializer.Serialize(writer, questionAnswer.QuestionType);
            writer.WritePropertyName("answer");
            serializer.Serialize(writer, questionAnswer.GetAnswer());
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteNull();
        }
    }

    private static QuestionAnswer Create(JObject jsonObject)
    {
        var typeToken = jsonObject.GetValue("questionType", StringComparison.OrdinalIgnoreCase);
        if (typeToken is null || typeToken.Type is not (JTokenType.String or JTokenType.Integer) ||
            !Enum.TryParse<QuestionType>(typeToken.ToString(), true, out var questionType) ||
            !Enum.IsDefined(typeof(QuestionType), questionType))
            throw new JsonSerializationException("A valid questionType is required (Text, Number or Choice).");

        var codeToken = jsonObject.GetValue("questionCode", StringComparison.OrdinalIgnoreCase);
        var answerToken = jsonObject.GetValue("answer", StringComparison.OrdinalIgnoreCase);
        if (codeToken?.Type != JTokenType.String || string.IsNullOrWhiteSpace(codeToken.Value<string>()) ||
            answerToken is null || answerToken.Type == JTokenType.Null)
            throw new JsonSerializationException("Each question requires a questionCode and an answer.");

        QuestionAnswer answer = questionType switch
        {
            QuestionType.Text when answerToken.Type == JTokenType.String =>
                new TextQuestionAnswer { Answer = answerToken.Value<string>() },
            QuestionType.Choice when answerToken.Type == JTokenType.String =>
                new ChoiceQuestionAnswer { Answer = answerToken.Value<string>() },
            QuestionType.Number => new NumericQuestionAnswer { Answer = ReadNumber(answerToken) },
            _ => throw new JsonSerializationException("Text and choice answers must be JSON strings.")
        };
        answer.QuestionCode = codeToken.Value<string>();
        return answer;
    }

    private static decimal ReadNumber(JToken token)
    {
        if ((token.Type is JTokenType.Integer or JTokenType.Float or JTokenType.String) &&
            decimal.TryParse(token.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
            return value;
        throw new JsonSerializationException("A number answer must contain a decimal value.");
    }
}
