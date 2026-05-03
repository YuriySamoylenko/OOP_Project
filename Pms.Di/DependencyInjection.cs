using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pms.Bll.Interfaces;
using Pms.Bll.Services;
using Pms.Core.Entities;
using Pms.Core.Interfaces;
using Pms.Dal;
using Pms.Data;

namespace Pms.Di
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPmsInfrastructure(this IServiceCollection services, string connectionString)
        {
            // 1. Database Setup (SQLite)
            services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlite(connectionString), ServiceLifetime.Scoped);

            services.AddDatabaseDeveloperPageExceptionFilter();

            // 2. Identity Setup (Note the <User> and <int> types we discussed)
            services.AddIdentityCore<User>()
                .AddRoles<IdentityRole>() // Support for UserRole enum logic
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            services.AddDatabaseDeveloperPageExceptionFilter();

            // 4. Register your Repositories and Services
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IDataTransfer, DataTransfer>();
            services.AddSingleton<INotificationService, NotificationService>();
            // services.AddScoped<IUserInProjectService, UserInProjectService>();

            return services;
        }
    }
}
