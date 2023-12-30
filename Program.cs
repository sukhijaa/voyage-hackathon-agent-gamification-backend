
using dynamoDb.model;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Agent_Gamification
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS services
            builder.Services.AddCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use CORS middleware
            app.UseCors(builder => builder
    .WithOrigins("http://localhost:3000", "http://localhost:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

            app.MapPost("/AddAwardAfterBooking", (AddAwardAfterBookingRQ pAddAwardAfterBooking) =>
            {
                AddAwardAfterBookingRS response = new AddAwardAfterBookingRS();
                try
                {
                    Award award = new Award();
                    response = award.AddAwardAfterBooking(pAddAwardAfterBooking);
                }
                catch (Exception ex)
                {

                }
                return response;

            }).WithOpenApi();

            app.MapGet("/GetNextMileStone", ([FromBody] NextMileStoneRQ pNextMileStoneRQ) =>
            {
                NextMileStoneRS response = new NextMileStoneRS();
                try
                {
                    Award award = new Award();
                    response = award.GetNextMileStone(pNextMileStoneRQ);
                }
                catch (Exception ex)
                {

                }
                return response;

            }).WithOpenApi();

            app.MapGet("/GetLeaderboardRankings", ([FromBody] LeaderboardRankingRQ pLeaderboardRankingRQ) =>
            {
                LeaderboardRankingRS response = new LeaderboardRankingRS();
                try
                {
                    Award award = new Award();
                    response = award.GetLeaderboardRankings(pLeaderboardRankingRQ);
                }
                catch (Exception ex)
                {

                }
                return response;

            }).WithOpenApi();

            app.MapGet("/GetAwardHistory", ([FromBody] AwardHistoryRQ pAwardHistoryRQ) =>
            {
                AwardHistoryRS response = new AwardHistoryRS();
                try
                {
                    Award award = new Award();
                    response = award.GetAwardHistory(pAwardHistoryRQ);
                }
                catch (Exception ex)
                {

                }
                return response;


            }).WithOpenApi();

            app.MapPost("/ReedimAwardPoints", (ReedimAwardPointsRQ pReedimAwardPointsRQ) =>
            {
                ReedimAwardPointsRS response = new ReedimAwardPointsRS();
                try
                {
                    Award award = new Award();
                    response = award.ReedimAwardPoints(pReedimAwardPointsRQ);
                }
                catch (Exception ex)
                {

                }
                return response;

            }).WithOpenApi();

            app.MapGet("/GetRedemptionHistory", ([FromBody] RedemptionHistoryRQ pAwardHistoryRQ) =>
            {
                List<RedemptionHistory> response = new List<RedemptionHistory>();
                try
                {
                    Award award = new Award();
                    response = award.GetRedemptionHistory(pAwardHistoryRQ);
                }
                catch (Exception ex)
                {

                }
                return response;


            }).WithOpenApi();

            app.MapGet("/GetAgentProfile", ([FromBody] AgentProfileRQ pAgentProfileRQ) =>
            {
                List<AgentProfile> response = new List<AgentProfile>();
                try
                {
                    Award award = new Award();
                    response = award.GetAgentProfile(pAgentProfileRQ);
                }
                catch (Exception ex)
                {

                }
                return response;

            }).WithOpenApi();

            app.Run();
        }

    }
}
