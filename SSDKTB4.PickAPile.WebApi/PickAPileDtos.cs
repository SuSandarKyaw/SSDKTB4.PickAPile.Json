using SSDKTB4.PickAPile.WebApi.Services;

namespace SSDKTB4.PickAPile.WebApi
{
	public class QuestionDto
	{
		public int QuestionId { get; set; }
		public string QuestionName { get; set; }
		public string QuestionDesp { get; set; }
		public List<AnswerDto> Answers { get; set; } = new();
	}

	
	public class AnswerDto
	{
		public int AnswerId { get; set; }
		public string AnswerImageUrl { get; set; }
		public string AnswerName { get; set; }
		public string AnswerDesp { get; set; }
		public int QuestionId { get; set; }
	}


	
	
	public class PickAPileResponseDto
	{
		public List<QuestionDto> Questions { get; set; }
	}
}
