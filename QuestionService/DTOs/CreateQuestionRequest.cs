namespace QuestionService.DTOs;

public class CreateQuestionRequest
{
    [Required] public required string Title { get; set; }
    [Required] public required string Content { get; set; }
    [Required] [TagListValidator(1, 5)] public List<string> Tags { get; set; } = [];
}