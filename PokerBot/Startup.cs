using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokerBot.Repository;
using PokerBot.Models;
using Microsoft.EntityFrameworkCore;
using PokerBot.Services;
using PokerBot.Repository.Mavens;

namespace PokerBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<ISecrets, Secrets>();
            services.AddScoped<IPokerRepository, PokerRepository>();
            services.AddScoped<ISlackClient, SlackClient>();
            services.AddSingleton<IGameState, GameState>();
            services.AddHttpClient<IMavenAccountsEdit, MavenAccountsEdit>();
            services.AddHttpClient<IMavenAccountsAdd, MavenAccountsAdd>();
            services.AddHttpClient<IMavenAccountsList, MavenAccountsList>();
            services.AddHttpClient<IMavenLogsHandHistory, MavenLogsHandHistory>();
            services.AddHttpClient<IMavenRingGamesList, MavenRingGamesList>();
            services.AddHttpClient<IMavenRingGamesGet, MavenRingGamesGet>();
            services.AddHttpClient<IMavenRingGamesMessage, MavenRingGamesMessage>();
            services.AddHttpClient<IMavenRingGamesPlaying, MavenRingGamesPlaying>();
            services.AddHttpClient<IMavenTournamentsPlaying, MavenTournamentsPlaying>();
            services.AddHttpClient<IMavenTournamentsWaiting, MavenTournamentsWaiting>();
            services.AddHttpClient<IMavenTournamentsList, MavenTournamentsList>();

            //background services
            services.AddHostedService<ConsumeScopedBalance>();
            services.AddScoped<IScopedBalance, ScopedBalance>();
            services.AddHostedService<ConsumeSlackUserAvatar>();
            services.AddScoped<ISlackUserAvatar, SlackUserAvatar>();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                services.AddDbContext<PokerDBContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("azureConnection")));
            else
                services.AddDbContext<PokerDBContext>(options =>
                        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
