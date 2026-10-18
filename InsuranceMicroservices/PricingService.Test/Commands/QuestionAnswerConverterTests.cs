using Newtonsoft.Json;
using PricingService.Api.Commands.Dto;
using Xunit;

namespace PricingService.Test.Commands;

public class QuestionAnswerConverterTests
{
    [Fact]
    public void NumericAnswerPreservesDecimalValueOnRoundTrip()
    {
        QuestionAnswer original = new NumericQuestionAnswer { QuestionCode = "AREA", Answer = 95.25M };
        var json = JsonConvert.SerializeObject(original);
        var restored = JsonConvert.DeserializeObject<QuestionAnswer>(json);
        var number = Assert.IsType<NumericQuestionAnswer>(restored);
        Assert.Equal("AREA", number.QuestionCode);
        Assert.Equal(95.25M, number.Answer);
    }

    [Fact]
    public void AcceptsPascalCasePropertiesAndChoiceAnswers()
    {
        const string json = "{\"QuestionCode\":\"FLOOD\",\"QuestionType\":\"Choice\",\"Answer\":\"YES\"}";
        var answer = Assert.IsType<ChoiceQuestionAnswer>(JsonConvert.DeserializeObject<QuestionAnswer>(json));
        Assert.Equal("YES", answer.Answer);
    }

    [Theory]
    [InlineData("{\"questionCode\":\"AREA\",\"questionType\":\"Number\"}")]
    [InlineData("{\"questionCode\":\"AREA\",\"questionType\":\"Unknown\",\"answer\":1}")]
    [InlineData("{\"questionCode\":\"AREA\",\"questionType\":\"Number\",\"answer\":\"invalid\"}")]
    public void InvalidAnswerProducesASerializationError(string json)
    {
        Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<QuestionAnswer>(json));
    }
}
