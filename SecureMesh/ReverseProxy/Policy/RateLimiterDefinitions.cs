using System;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace SecureMesh.ReverseProxy.Policy;

public static class RateLimiterDefinitions
{
    public static void ConfigureRateLimiting(this RateLimiterOptions options)
    {
        /// MORE SENSITIVE        
        
        // register 
        options.AddFixedWindowLimiter("register", op =>
        {
            op.PermitLimit = 2;
            op.Window = TimeSpan.FromMinutes(10);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        //login
        options.AddFixedWindowLimiter("login-rs", op =>
        {
            op.PermitLimit = 2;
            op.Window = TimeSpan.FromMinutes(30);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // forget password
        options.AddFixedWindowLimiter("forget-password", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(20);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // reset password
        options.AddFixedWindowLimiter("reset-password", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(20);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Verify Email 
        options.AddFixedWindowLimiter("verify-email", op =>
        {
            op.PermitLimit = 2;
            op.Window = TimeSpan.FromMinutes(30);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Verify New Email 
        options.AddFixedWindowLimiter("verify-new-email", op =>
        {
            op.PermitLimit = 2;
            op.Window = TimeSpan.FromMinutes(30);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        /// LESS SENSITIVE + Queueing ALLOWED        
        
        /// //logout
        options.AddFixedWindowLimiter("logout", op =>
        {
            op.PermitLimit = 4;
            op.Window = TimeSpan.FromMinutes(6);
            op.QueueLimit = 1;
            op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // password update
        options.AddFixedWindowLimiter("password-update", op =>
        {
            op.PermitLimit = 2;
            op.Window = TimeSpan.FromMinutes(30);
            op.QueueLimit = 1;
            op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // update credentials (Username, fullname)
        options.AddFixedWindowLimiter("update-credentials", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(30);
            op.QueueLimit = 1;
            op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // update any credentials (Username, fullname)
        options.AddFixedWindowLimiter("update-any-credentials", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(30);
            op.QueueLimit = 1;
            op.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // update email
        options.AddFixedWindowLimiter("update-email", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(30);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // remove user
        options.AddFixedWindowLimiter("remove-user", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(30);
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    }
}
