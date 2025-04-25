using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Security.Module.DTOs;
using Security.Module.Filter;
using Security.Module.Services.Interfaces;
using SecurityTesting.Mock;

namespace SecurityTesting.Filters;

public class UnitLocalAuthFilter
{
    /// <summary>
    /// Local Auth Filter
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldValidateTheUserCredentialAsFilterInController()
    {
        var service = new Mock<ISecurityService>();
        var body = SecurityMock.LoginDTOMock;
        var user = SecurityMock.UserHashPassMock;

        service.Setup(s => s.ValidateUser(It.IsAny<LoginDTO>())).ReturnsAsync(user);

        var filter = new LocalAuthFilter(service.Object);
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor(), new ModelStateDictionary());

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { { "body", body } },
            new object()
        );

        var executedAction = new ActionExecutedContext(
            actionContext,
            new List<IFilterMetadata>(),
            context.Controller
        );

        await filter.OnActionExecutionAsync(context, () => Task.FromResult(executedAction));

        Assert.Equal(user, context.HttpContext.Items["User"]);
    }
}
