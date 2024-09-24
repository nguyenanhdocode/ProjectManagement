using Application.Common;
using Application.MappingProfiles;
using Application.Services;
using Application.Services.DevImpl;
using Application.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services
            , IConfiguration configuration, IWebHostEnvironment env)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection("JwtConfiguration"));
            services.Configure<SmtpConfiguration>(configuration.GetSection("SmtpConfiguration"));
            services.Configure<CloudinaryConfiguration>(configuration.GetSection("CloudinaryConfiguration"));
            services.Configure<StaticConfiguration>(configuration.GetSection("StaticConfiguration"));
            services.Configure<AppConfiguration>(configuration.GetSection("AppConfiguration"));

            services.AddAutoMapper(typeof(IMappingProfilesMarker));
            
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IAssetService, FileService>();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();

            if (env.IsProduction())
                services.AddScoped<IEmailService, EmailService>();
            else
                services.AddScoped<IEmailService, DevEmailService>();

            return services;
        }
    }
}
