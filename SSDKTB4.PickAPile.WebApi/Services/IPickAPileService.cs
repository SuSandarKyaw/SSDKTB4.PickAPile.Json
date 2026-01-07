
namespace SSDKTB4.PickAPile.WebApi.Services
{
    public interface IPickAPileService
    {
        Task<PileListsResponseModel> GetAllPilesAsync();
        Task<PileItemResponseModel> GetPileByIdAsync(int id);
        Task<SearchPileResponseModel> SearchPilesAsync(string query);
    }
}