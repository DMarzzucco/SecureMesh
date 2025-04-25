using Microsoft.AspNetCore.Http;
using Moq;
using Security.Cookies.Interfaces;
using Security.JWT.Interfaces;
using Security.Module.Services.Interfaces;
using Security.Utils.Middleware;
using SecurityTesting.Mock;

namespace SecurityTesting.Middleware;

public class UnitRefreshTokenMIddlewareTest
{
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<ISecurityService> _securityService;
    private readonly Mock<ICookieService> _cookieService;
    private readonly RequestDelegate _next;

    public UnitRefreshTokenMIddlewareTest()
    {
        this._jwtService = new Mock<IJwtService>();
        this._securityService = new Mock<ISecurityService>();
        this._cookieService = new Mock<ICookieService>();
        this._next = (HttpContext context) => Task.CompletedTask;
    }

    /// <summary>
    /// Allow Public Path
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldAllowPublicPathAndContinue()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/Security/login";

        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._securityService.Object, this._cookieService.Object);

        Assert.Equal(200, context.Response.StatusCode);
    }

    /// <summary>
    /// Return 401 if Access Token is Missing
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturn401WhenAccessTokenIsMissing()
    {
        var context = new DefaultHttpContext();
        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._securityService.Object, this._cookieService.Object);

        Assert.Equal(401, context.Response.StatusCode);
    }

    /// <summary>
    /// Return 403 if Access Token is a Invalid token
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldReturn403WhenAccessTokenIsInvalid()
    {
        var context = new DefaultHttpContext();

        context.Request.Headers["Cookie"] = "Authentication=InvalidToken";

        this._jwtService.Setup(j => j.ValidateToken("invalid")).Returns(false);

        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._securityService.Object, this._cookieService.Object);

        Assert.Equal(403, context.Response.StatusCode);
    }

    /// <summary>
    /// Should Refresh Toke if his expiration is soon
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ShouldRefreshTokenWhenExpirationIsSoon()
    {
        var context = new DefaultHttpContext();
        var token = SecurityMock.TokenMock;

        context.Request.Headers["Cookie"] = "Authentication=validToken; RefreshToken=validRefreshToken";

        var user = SecurityMock.UserMock;

        this._jwtService.Setup(j => j.ValidateToken("validToken")).Returns(true);
        this._jwtService.Setup(j => j.isTokenExpirationSoon("validToken")).Returns(true);
        this._jwtService.Setup(j => j.ValidateToken("validRefreshToken")).Returns(true);

        this._securityService.Setup(s => s.GetUserByCookie()).ReturnsAsync(user);

        this._jwtService.Setup(j => j.RefreshToken(user)).Returns(token);

        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._securityService.Object, this._cookieService.Object);

        this._cookieService.Verify(c => c.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(200, context.Response.StatusCode);
    }
}
