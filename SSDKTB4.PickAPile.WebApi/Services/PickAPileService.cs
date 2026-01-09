using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace SSDKTB4.PickAPile.WebApi.Services;

public class PickAPileService : IPickAPileService
{
	private readonly IDbConnection _db;

	public PickAPileService(IDbConnection db)
	{
		_db = db;
	}
	public async Task<PileListsResponseModel> GetAllPilesAsync()
	{

		string questionSql = "SELECT * FROM Questions";
		var questions = (await _db.QueryAsync<QuestionDto>(questionSql)).ToList();

		string answerSql = "SELECT * FROM Answers";
		var answers = (await _db.QueryAsync<AnswerDto>(answerSql)).ToList();


		foreach (var q in questions)
		{
			q.Answers = answers.Where(a => a.QuestionId == q.QuestionId).ToList();

		}
		return new PileListsResponseModel
		{
			IsSuccess = true,
			Message = "Piles retrieved successfully.",
			Data = questions
		};

	}


	public async Task<PileItemResponseModel> GetPileByIdAsync(int id)
	{


		string questionSql = @"
            SELECT * FROM Questions
            WHERE QuestionId = @QuestionId";

		var question = await _db.QueryFirstOrDefaultAsync<QuestionDto>(
			questionSql,
			new { QuestionId = id }
		);

		if (question is null)
		{

			return new PileItemResponseModel
			{
				IsSuccess = false,
				Message = "Pile not found.",

			};
		}
		string answerSql = @"
            SELECT * FROM Answers
            WHERE QuestionId = @QuestionId";

		var answers = (await _db.QueryAsync<AnswerDto>(
			answerSql,
			new { QuestionId = id }
		)).ToList();

		question.Answers = answers;

		return new PileItemResponseModel
		{
			IsSuccess = true,
			Message = "Pile retrieved successfully.",
			Data = question
		};



	}

	public async Task<SearchPileResponseModel> SearchPilesAsync(string query)
	{


		string searchTerm = $"%{query}%";

		string questionSql = @"
           SELECT DISTINCT q.* FROM Questions q
            LEFT JOIN Answers a ON q.QuestionId = a.QuestionId
            WHERE q.QuestionName LIKE @Search 
               OR q.QuestionDesp LIKE @Search
               OR a.AnswerDesp LIKE @Search
               OR a.AnswerName LIKE @Search";

		var questions = (await _db.QueryAsync<QuestionDto>(
			questionSql,
			new { Search = searchTerm }
		)).ToList();

		if (!questions.Any())
		{
			return new SearchPileResponseModel
			{
				IsSuccess = false,
				Message = "No matching piles found.",
			};
		}


		string answerSql = @"
            SELECT * FROM Answers 
            WHERE QuestionId IN @QuestionIds";

		var answers = (await _db.QueryAsync<AnswerDto>(
			answerSql,
			new { QuestionIds = questions.Select(q => q.QuestionId) }
		)).ToList();

		foreach (var q in questions)
		{
			q.Answers = answers.Where(a => a.QuestionId == q.QuestionId).ToList();

		}
		return new SearchPileResponseModel
		{
			IsSuccess = true,
			Message = "Piles retrieved successfully.",
			Data = questions
		};
	}

}


public class PileListsResponseModel
{
	public bool IsSuccess { get; set; }
	public string Message { get; set; }
	public List<QuestionDto> Data { get; set; }
}
public class PileItemResponseModel
{
	public bool IsSuccess { get; set; }
	public string Message { get; set; }
	public QuestionDto Data { get; set; }
}

public class SearchPileResponseModel
{
	public bool IsSuccess { get; set; }
	public string Message { get; set; }
	public List<QuestionDto> Data { get; set; }
}
