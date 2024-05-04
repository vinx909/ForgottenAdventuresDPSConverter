using ForgottenAdventuresDPSConverter.Core.Entities;
using ForgottenAdventuresDPSConverter.Core.Interfaces;
using ForgottenAdventuresDPSConverter.Core.Services;
using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.Interfaces;
using ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Settings;
using ForgottenAdventuresDPSConverter.FileRepository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF
{
    internal static class ServiceProviderProvider
    {
        internal static IServiceProvider SetupServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services
                .AddScoped<IDpsFolderService, DpsFolderService>()
                .AddScoped<IDpsNumberService, DpsNumberService>()
                .AddScoped<IDpsSubfolderService, DpsSubfolderService>()
                .AddScoped<IFAFolderService, FAFolderService>()
                .AddScoped<ICommandsService, CommandsService>()
                .AddScoped<IHtmlConverter, HtmlConverter>()
                .AddScoped<IWallConverter, WallConverter>()
                .AddScoped<IConverterService, ConverterService>();

            services
                .AddScoped<ISettingsGetter, SettingsGetter>();

            services
                .AddScoped<IFileRepositorySettings, FileRepositorySettingsFromSettings>()
                .AddScoped<IRepository<DpsFolder>, ForgottenAdventuresDPSConverter.FileRepository.DpsFolderFileRepository>()
                .AddScoped<IRepository<DpsNumber>, ForgottenAdventuresDPSConverter.FileRepository.DpsNumberFileRepository>()
                .AddScoped<IRepository<DpsSubfolder>, ForgottenAdventuresDPSConverter.FileRepository.DpsSubfolderFileRepository>()
                .AddScoped<IRepository<FAFolder>, ForgottenAdventuresDPSConverter.FileRepository.FAFolderFileRepository>();

            return services.BuildServiceProvider();
        }
    }
}
