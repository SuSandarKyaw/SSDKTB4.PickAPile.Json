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
    private readonly IConfiguration _configuration;
    public PickAPileService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private string GetConnection() => _configuration.GetConnectionString("DbConnection");
    public async Task<PileListsResponseModel> GetAllPilesAsync()
    {
        using (IDbConnection db = new SqlConnection(GetConnection()))
        {

            string questionSql = "SELECT * FROM Questions";
            var questions = (await db.QueryAsync<QuestionDto>(questionSql)).ToList();

            string answerSql = "SELECT * FROM Answers";
            var answers = (await db.QueryAsync<AnswerDto>(answerSql)).ToList();

            var questionDtoList = new List<QuestionDto>();

            foreach (var question in questions)
            {
                var dto = new QuestionDto
                {
                    QuestionId = question.QuestionId,
                    QuestionName = question.QuestionName,
                    QuestionDesp = question.QuestionDesp,
                    Answers = answers
                        .Where(a => a.QuestionId == question.QuestionId)
                        .Select(a => new AnswerDto
                        {
                            AnswerId = a.AnswerId,
                            AnswerName = a.AnswerName,
                            AnswerDesp = a.AnswerDesp,
                            AnswerImageUrl = a.AnswerImageUrl,
                            QuestionId = a.QuestionId
                        })
                        .ToList()
                };
                questionDtoList.Add(dto);
            }
            return new PileListsResponseModel
            {
                IsSuccess = true,
                Message = "Piles retrieved successfully.",
                Data = questionDtoList
            };
        }
    }


    public async Task<PileItemResponseModel> GetPileByIdAsync(int id)
    {
        using (IDbConnection db = new SqlConnection(GetConnection()))
        {

            string questionSql = @"
            SELECT * FROM Questions
            WHERE QuestionId = @QuestionId";

            var question = await db.QueryFirstOrDefaultAsync<QuestionDto>(
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

            var answers = (await db.QueryAsync<AnswerDto>(
                answerSql,
                new { QuestionId = id }
            )).ToList();
            var questionDto = new QuestionDto
            {
                QuestionId = question.QuestionId,
                QuestionName = question.QuestionName,
                QuestionDesp = question.QuestionDesp,
                Answers = answers.Select(a => new AnswerDto
                {
                    AnswerId = a.AnswerId,
                    AnswerName = a.AnswerName,
                    AnswerDesp = a.AnswerDesp,
                    AnswerImageUrl = a.AnswerImageUrl,
                    QuestionId = a.QuestionId
                }).ToList()
            };

            return new PileItemResponseModel
            {
                IsSuccess = true,
                Message = "Pile retrieved successfully.",
                Data = questionDto
            };
        }


    }

    public async Task<SearchPileResponseModel> SearchPilesAsync(string query)
    {
        using (IDbConnection db = new SqlConnection(GetConnection()))
        {

            string searchTerm = $"%{query}%";

            string questionSql = @"
           SELECT DISTINCT q.* FROM Questions q
            LEFT JOIN Answers a ON q.QuestionId = a.QuestionId
            WHERE q.QuestionName LIKE @Search 
               OR q.QuestionDesp LIKE @Search
               OR a.AnswerDesp LIKE @Search
               OR a.AnswerName LIKE @Search";

            var questions = (await db.QueryAsync<QuestionDto>(
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

            var answers = (await db.QueryAsync<AnswerDto>(
                answerSql,
                new { QuestionIds = questions.Select(q => q.QuestionId) }
            )).ToList();

            var questionDtoList = new List<QuestionDto>();

            foreach (var question in questions)
            {
                var dto = new QuestionDto
                {
                    QuestionId = question.QuestionId,
                    QuestionName = question.QuestionName,
                    QuestionDesp = question.QuestionDesp,
                    Answers = answers
                        .Where(a => a.QuestionId == question.QuestionId)
                        .Select(a => new AnswerDto
                        {
                            AnswerId = a.AnswerId,
                            AnswerName = a.AnswerName,
                            AnswerDesp = a.AnswerDesp,
                            AnswerImageUrl = a.AnswerImageUrl,
                            QuestionId = a.QuestionId
                        })
                        .ToList()
                };
                questionDtoList.Add(dto);
            }
            return new SearchPileResponseModel
            {
                IsSuccess = true,
                Message = "Piles retrieved successfully.",
                Data = questionDtoList
            };
        }
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
