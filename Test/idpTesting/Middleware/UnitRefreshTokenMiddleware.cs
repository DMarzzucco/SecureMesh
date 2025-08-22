
using IdentifyService.Cookies.Interfaces;
using IdentifyService.JWT.Interfaces;
using IdentifyService.Module.DTOs;
using IdentifyService.Module.Services.Interfaces;
using IdentifyService.Utils.Middleware;
using idpTesting.Mock;
using Microsoft.AspNetCore.Http;
using Moq;

namespace idpTesting.Middleware;

public class UnitRefreshTokenMiddleware
{
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IIdentityProviderService> _idpService;
    private readonly Mock<ICookieService> _cookieService;
    private readonly RequestDelegate _next;

    public UnitRefreshTokenMiddleware()
    {
        this._jwtService = new Mock<IJwtService>();
        this._idpService = new Mock<IIdentityProviderService>();
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
        context.Request.Path = "/api/Idp/login";

        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._idpService.Object, this._cookieService.Object);

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

        await middleware.InvokeAsync(context, this._jwtService.Object, this._idpService.Object, this._cookieService.Object);

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

        this._jwtService.Setup(j => j.ValidateAuthenticationToken("invalid")).Returns(false);

        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._idpService.Object, this._cookieService.Object);

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
        var token = IdentityServiceMock.TokenMock;

        int sessionId = 1;

        context.Request.Headers["Cookie"] = "Authentication=validToken; RefreshToken=validRefreshToken";

        var user = IdentityServiceMock.UserMock;
        var payload = new AuthorizationTokenDTO { User = user, SessionId = sessionId };

        this._jwtService.Setup(j => j.ValidateAuthenticationToken("validToken")).Returns(true);
        this._jwtService.Setup(j => j.IsTokenExpirationSoon("validToken")).Returns(true);
        this._jwtService.Setup(j => j.ValidateAuthenticationToken("validRefreshToken")).Returns(true);

        this._idpService.Setup(s => s.GetValueByCookie()).ReturnsAsync(payload);

        this._jwtService.Setup(j => j.GenerateRefreshToken(sessionId, user)).Returns(token);

        var middleware = new RefreshTokenMiddleware(this._next);

        await middleware.InvokeAsync(context, this._jwtService.Object, this._idpService.Object, this._cookieService.Object);

        this._cookieService.Verify(c => c.SetTokenCookies(It.IsAny<HttpResponse>(), token), Times.Once);

        Assert.Equal(200, context.Response.StatusCode);
    }

}
