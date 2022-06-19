using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace IdentityApi.Services.CustomRazorEngine;

public class CustomRazorEngine : ICustomRazorEngine
{
    private readonly IRazorViewEngine _razorViewEngine; // used to render the pages that use razor syntax.
    private readonly IServiceProvider _serviceProvider; // provider for creating instances.
    private readonly ITempDataProvider _tempDataProvider; // temporary storage memory for subsequent request.

    public CustomRazorEngine(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _razorViewEngine = razorViewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RazorViewToHtmlAsync<TModel>(string viewName, TModel model)
    {
        var actionContext = GetContext();
        var view = FindView(viewName);

        using var output = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<TModel>(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary()
            ) { Model = model },
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            output,
            new HtmlHelperOptions()
        );
        await view.RenderAsync(viewContext);
        return output.ToString();
    }

    private IView FindView(string ViewName)
    {
        var viewResult = _razorViewEngine.GetView(null, ViewName, true);
        if (viewResult.Success)
        {
            return viewResult.View;
        }

        throw new Exception("Invalid View Path");
    }

    private ActionContext GetContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = _serviceProvider;
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }
}
