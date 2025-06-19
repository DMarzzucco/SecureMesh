using System;
using Microsoft.AspNetCore.RateLimiting;

namespace SecureMesh.ReverseProxy.Policy;

public static class RateLimiterDefinitions
{
    public static void ConfigureRateLimiting(this RateLimiterOptions options)
    {
        // basic rate limiting policy
        options.AddFixedWindowLimiter("rt-sl", op =>
        {
            op.PermitLimit = 2;
            op.Window = TimeSpan.FromMinutes(6);
            //op.QueueLimit = 1;
            //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        //login
        options.AddFixedWindowLimiter("login-rs", op =>
        {
            op.PermitLimit = 4;
            op.Window = TimeSpan.FromMinutes(6);
            //op.QueueLimit = 1;
            //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // password update policy
        options.AddFixedWindowLimiter("password-update", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(15);
            //op.QueueLimit = 1;
            //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // update credentials (Username, fullname, email)
        options.AddFixedWindowLimiter("update-credentials", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromDays(60);
            //op.QueueLimit = 1;
            //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // Workers (Email Verification & Password recuperation)
        options.AddFixedWindowLimiter("workers-limit", op =>
        {
            op.PermitLimit = 1;
            op.Window = TimeSpan.FromMinutes(20);
            //op.QueueLimit = 1;
            //op.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    }
}
