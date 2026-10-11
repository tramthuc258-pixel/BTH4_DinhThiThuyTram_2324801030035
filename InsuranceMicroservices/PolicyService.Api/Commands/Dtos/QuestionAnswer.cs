namespace PolicyService.Api.Commands.Dtos;

public enum QuestionType
{
    Numeric,
    Choice
}

public abstract class QuestionAnswer
{
    public string QuestionCode { get; set; } = string.Empty;
    public abstract QuestionType QuestionType { get; }
}

public class NumericQuestionAnswer : QuestionAnswer
{
    public override QuestionType QuestionType => QuestionType.Numeric;
    public decimal Answer { get; set; }
}

public class ChoiceQuestionAnswer : QuestionAnswer
{
    public override QuestionType QuestionType => QuestionType.Choice;
    public string Answer { get; set; } = string.Empty;
}
