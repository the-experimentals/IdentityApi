using System.Threading.Tasks;

namespace IdentityApi.Services.CustomRazorEngine;

public interface ICustomRazorEngine
{
    Task<string> RazorViewToHtmlAsync<TModel>(string viewName, TModel model);
}
