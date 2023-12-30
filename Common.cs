using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon;
using dynamoDb.model;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Agent_Gamification;
using System.Text.Json;
using System.Runtime.Serialization;

public class Dynamo
{
    private static IDynamoDBContext dynamoDBContext;
    private static AmazonDynamoDBClient client;
    public static IDynamoDBContext DynamoDBContext
    {
        get
        {
            try
            {
                //AWSCredentials credentials = new BasicAWSCredentials("DynamoDBAccessKey", "DynamoDBSecretKey");
                //AmazonDynamoDBConfig config = new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.GetBySystemName("DynamoDBRegion") };
                //client = new AmazonDynamoDBClient(credentials, config);
                //dynamoDBContext = new DynamoDBContext(client);

                AWSCredentials credentials = FallbackCredentialsFactory.GetCredentials();
                AmazonDynamoDBConfig config = new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.GetBySystemName("ap-south-1") };
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, config);
                dynamoDBContext = new DynamoDBContext(client);
            }
            catch (Exception ex)
            {

            }
            return dynamoDBContext;
        }
    }
}

public class Award
{
    public AddAwardAfterBookingRS AddAwardAfterBooking(AddAwardAfterBookingRQ pAddAwardAfterBookingRQ)
    {
        AddAwardAfterBookingRS addAwardAfterBookingRS = new AddAwardAfterBookingRS();
        try
        {
            var tasks = new List<Task>();
            AgentProfile agentProfile = new AgentProfile();
            string upgradedBadgeName = string.Empty;

            // Get Agent Profile
            var conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<AgentProfile> allAgentProfile = Dynamo.DynamoDBContext.ScanAsync<AgentProfile>(conditions).GetRemainingAsync().Result;
            agentProfile = allAgentProfile[0];

            conditions = new List<ScanCondition>();
            // you can add scan conditions, or leave empty
            List<AwardRules> allRules = Dynamo.DynamoDBContext.ScanAsync<AwardRules>(conditions).GetRemainingAsync().Result;

            // Get Award History
            conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<AwardHistory> agentAwardHistory = Dynamo.DynamoDBContext.ScanAsync<AwardHistory>(conditions).GetRemainingAsync().Result;

            // Get BadgeRule
            conditions = new List<ScanCondition>();
            //conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<BadgeRule> BadgeRules = Dynamo.DynamoDBContext.ScanAsync<BadgeRule>(conditions).GetRemainingAsync().Result;

            int totalEarnedPoints = 0;
            int earnedPoints = 0;

            foreach (AwardRules record in allRules)
            {
                AwardHistory awardHistory = new AwardHistory();
                awardHistory.AgentCode_AwardRuleCode_BookingConfirmationNo = pAddAwardAfterBookingRQ.AgentCode + "_" + record.RuleCode + "_" + pAddAwardAfterBookingRQ.BookingConfirmationNo;

                var distinctTypeIDs = agentAwardHistory.FirstOrDefault(x => x.AgentCode_AwardRuleCode_BookingConfirmationNo == awardHistory.AgentCode_AwardRuleCode_BookingConfirmationNo);
                //|| ((List<AwardHistory>)distinctTypeIDs).Count <= 0
                if (distinctTypeIDs == null)
                {

                    //1 Every Booking -50
                    //2 Every 100th Booking -100
                    //3 Every 500th Booking -200
                    //5 Every 500th Booking in Single Month -200
                    //6 New Region Booking - 200

                    // Per booking
                    if (record.RuleCode == "AWD001")
                    {
                        if ((Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * (record.RulePoint * .01)) <= 0))
                            earnedPoints = 1;
                        else
                            earnedPoints = Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * (record.RulePoint * .01));

                        awardHistory.AgentCode = pAddAwardAfterBookingRQ.AgentCode;
                        awardHistory.AwardRuleCode = record.RuleCode;
                        awardHistory.AwardRuleName = record.RuleName;
                        awardHistory.BookingConfirmationNo = pAddAwardAfterBookingRQ.BookingConfirmationNo;
                        awardHistory.BookingDate = DateTime.UtcNow.ToLongDateString();
                        awardHistory.BookingAmount = pAddAwardAfterBookingRQ.BookingAmount;
                        awardHistory.EarnedPoint = earnedPoints;
                        agentProfile.TotalPoint += earnedPoints;
                        agentProfile.CurrentPoint += earnedPoints;
                        totalEarnedPoints += earnedPoints;
                        awardHistory.CityName = pAddAwardAfterBookingRQ.CityName;
                        awardHistory.CountryName = pAddAwardAfterBookingRQ.CountryName;
                        awardHistory.CountryName = pAddAwardAfterBookingRQ.CountryName;
                        awardHistory.LastModifyON = DateTime.Now.ToString();

                        tasks.Add(Dynamo.DynamoDBContext.SaveAsync(awardHistory));
                    }
                    //Every 10th Booking - 100
                    else if (record.RuleCode == "AWD002")
                    {
                        if ((Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * (record.RulePoint * .01))) <= 0)
                            earnedPoints = 1;
                        else
                            earnedPoints = Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * (record.RulePoint * .01));

                        var distinctType = agentAwardHistory.FindAll(item => item.BookingConfirmationNo == pAddAwardAfterBookingRQ.BookingConfirmationNo).Distinct();
                        if (distinctType != null && distinctType.Count() > 0 && distinctType.Count() % 10 == 0)
                        {
                            awardHistory.AgentCode = pAddAwardAfterBookingRQ.AgentCode;
                            awardHistory.AwardRuleCode = record.RuleCode;
                            awardHistory.AwardRuleName = record.RuleName;
                            awardHistory.BookingConfirmationNo = pAddAwardAfterBookingRQ.BookingConfirmationNo;
                            awardHistory.BookingAmount = pAddAwardAfterBookingRQ.BookingAmount;
                            awardHistory.BookingDate = DateTime.UtcNow.ToLongDateString();
                            awardHistory.EarnedPoint = earnedPoints;
                            agentProfile.TotalPoint += earnedPoints;
                            agentProfile.CurrentPoint += earnedPoints;
                            totalEarnedPoints += earnedPoints;
                            awardHistory.CityName = pAddAwardAfterBookingRQ.CityName;
                            awardHistory.CountryCode = pAddAwardAfterBookingRQ.CountryCode;
                            awardHistory.CountryName = pAddAwardAfterBookingRQ.CountryName;
                            awardHistory.LastModifyON = DateTime.Now.ToString();

                            tasks.Add(Dynamo.DynamoDBContext.SaveAsync(awardHistory));
                        }
                    }
                    //Every New City
                    else if (record.RuleCode == "AWD003")
                    {
                        if ((Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * .01)) <= 0)
                            earnedPoints = 1;
                        else
                            earnedPoints = Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * (record.RulePoint * .01));
                        distinctTypeIDs = agentAwardHistory.FirstOrDefault(x => x.CityName == pAddAwardAfterBookingRQ.CityName);
                        //|| ((List<AwardHistory>)distinctTypeIDs).Count <= 0
                        if (distinctTypeIDs == null)
                        {
                            awardHistory.AgentCode = pAddAwardAfterBookingRQ.AgentCode;
                            awardHistory.AwardRuleCode = record.RuleCode;
                            awardHistory.AwardRuleName = record.RuleName;
                            awardHistory.BookingConfirmationNo = pAddAwardAfterBookingRQ.BookingConfirmationNo;
                            awardHistory.BookingAmount = pAddAwardAfterBookingRQ.BookingAmount;
                            awardHistory.BookingDate = DateTime.UtcNow.ToLongDateString();
                            awardHistory.EarnedPoint = earnedPoints;
                            agentProfile.TotalPoint += earnedPoints;
                            agentProfile.CurrentPoint += earnedPoints;
                            totalEarnedPoints += earnedPoints;
                            awardHistory.CityName = pAddAwardAfterBookingRQ.CityName;
                            awardHistory.CountryCode = pAddAwardAfterBookingRQ.CountryCode;
                            awardHistory.CountryName = pAddAwardAfterBookingRQ.CountryName;
                            awardHistory.LastModifyON = DateTime.Now.ToString();

                            tasks.Add(Dynamo.DynamoDBContext.SaveAsync(awardHistory));
                        }
                    }
                    //Every New Country
                    else if (record.RuleCode == "AWD004")
                    {
                        if ((Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * .01)) <= 0)
                            earnedPoints = 1;
                        else
                            earnedPoints = Convert.ToInt32(pAddAwardAfterBookingRQ.BookingAmount * (record.RulePoint * .01));

                        distinctTypeIDs = agentAwardHistory.FirstOrDefault(x => x.CountryName == pAddAwardAfterBookingRQ.CountryName);
                        if (distinctTypeIDs == null)
                        {
                            awardHistory.AgentCode = pAddAwardAfterBookingRQ.AgentCode;
                            awardHistory.AwardRuleCode = record.RuleCode;
                            awardHistory.AwardRuleName = record.RuleName;
                            awardHistory.BookingConfirmationNo = pAddAwardAfterBookingRQ.BookingConfirmationNo;
                            awardHistory.BookingAmount = pAddAwardAfterBookingRQ.BookingAmount;
                            awardHistory.BookingDate = DateTime.UtcNow.ToLongDateString();
                            awardHistory.EarnedPoint = earnedPoints;
                            agentProfile.TotalPoint += earnedPoints;
                            agentProfile.CurrentPoint += earnedPoints;
                            totalEarnedPoints += earnedPoints;
                            awardHistory.CityName = pAddAwardAfterBookingRQ.CityName;
                            awardHistory.CountryCode = pAddAwardAfterBookingRQ.CountryCode;
                            awardHistory.CountryName = pAddAwardAfterBookingRQ.CountryName;
                            awardHistory.LastModifyON = DateTime.Now.ToString();

                            tasks.Add(Dynamo.DynamoDBContext.SaveAsync(awardHistory));
                        }
                    }
                }
            }
            BadgeRules = BadgeRules.OrderBy(x => x.RequiredPoint).ToList();
            foreach (BadgeRule BadgeRule in BadgeRules)
            {
                if (agentProfile.TotalPoint >= BadgeRule.RequiredPoint && agentProfile.BadgeName != BadgeRule.BadgeName)
                {
                    agentProfile.BadgeName = BadgeRule.BadgeName;
                    upgradedBadgeName = agentProfile.BadgeName;
                }
            }

            Task.WhenAll(tasks).GetAwaiter().GetResult();
            bool isSucces = true;
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].IsFaulted && tasks[i].Exception != null)
                {
                    isSucces = false;
                }
            }

            // Save Agent Updated Profile
            Dynamo.DynamoDBContext.SaveAsync<AgentProfile>(agentProfile).GetAwaiter().GetResult();


            if (isSucces)
            {
                addAwardAfterBookingRS.AgentCode = pAddAwardAfterBookingRQ.AgentCode;
                addAwardAfterBookingRS.StatusCode = StatusCodes.Sussess;
                addAwardAfterBookingRS.EarnedPoint = totalEarnedPoints;
                addAwardAfterBookingRS.TotalPoint = agentProfile.TotalPoint;
                if (!string.IsNullOrEmpty(upgradedBadgeName))
                    addAwardAfterBookingRS.UpgradedBadgeName = upgradedBadgeName;
            }
            else
            {
                addAwardAfterBookingRS.AgentCode = pAddAwardAfterBookingRQ.AgentCode;
                addAwardAfterBookingRS.StatusCode = StatusCodes.Failed;
                addAwardAfterBookingRS.ErrorCode = ErrrCodes.PrcessingError;
                addAwardAfterBookingRS.Error = "Failed While Updateing ";
            }

        }
        catch (Exception ex)
        {
            addAwardAfterBookingRS = new AddAwardAfterBookingRS { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message };
        }
        return addAwardAfterBookingRS;
    }

    public NextMileStoneRS GetNextMileStone(NextMileStoneRQ pNextMileStoneRQ)
    {
        NextMileStoneRS nextMileStoneRS = new NextMileStoneRS();
        try
        {
            nextMileStoneRS.RequiredPointsForTopClub = new RequiredPointsForTopClub();

            var conditions = new List<ScanCondition>();
            // conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<AgentProfile> allAgentProfile = Dynamo.DynamoDBContext.ScanAsync<AgentProfile>(conditions).GetRemainingAsync().Result;

            AgentProfile RequestedAgentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pNextMileStoneRQ.AgentCode);

            nextMileStoneRS.AgentCode = pNextMileStoneRQ.AgentCode;

            #region Global
            CalculateRank(allAgentProfile);
            AgentProfile agentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pNextMileStoneRQ.AgentCode);
            if (agentProfile != null)
                nextMileStoneRS.GlobalRank = agentProfile.Rank;

            if (nextMileStoneRS.GlobalRank != 1)
            {
                allAgentProfile = allAgentProfile.OrderByDescending(x => x.TotalPoint).ToList();
                agentProfile = allAgentProfile.FirstOrDefault();
                if (agentProfile != null)
                    nextMileStoneRS.RequiredPointsForTopClub.Top5GlobalClub = (agentProfile.TotalPoint - RequestedAgentProfile.TotalPoint) + 1;
            }
            #endregion

            #region City
            List<AgentProfile> singleCityAgents = allAgentProfile.FindAll(item => item.CityName == RequestedAgentProfile.CityName);
            CalculateRank(singleCityAgents);
            agentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pNextMileStoneRQ.AgentCode);
            if (agentProfile != null)
                nextMileStoneRS.CityRank = agentProfile.Rank;
            if (nextMileStoneRS.CityRank != 1)
            {
                singleCityAgents = singleCityAgents.OrderByDescending(x => x.TotalPoint).ToList();
                agentProfile = singleCityAgents.FirstOrDefault();
                if (agentProfile != null)
                    nextMileStoneRS.RequiredPointsForTopClub.Top5CityCub = (agentProfile.TotalPoint - RequestedAgentProfile.TotalPoint) + 1;
            }
            #endregion

            #region Country
            List<AgentProfile> singleCountryNameAgents = allAgentProfile.FindAll(item => item.CountryName == RequestedAgentProfile.CountryName);
            CalculateRank(singleCountryNameAgents);
            agentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pNextMileStoneRQ.AgentCode);
            if (agentProfile != null)
                nextMileStoneRS.CountryRank = agentProfile.Rank;

            if (nextMileStoneRS.CountryRank != 1)
            {
                singleCountryNameAgents = singleCountryNameAgents.OrderByDescending(x => x.TotalPoint).ToList();
                agentProfile = singleCountryNameAgents.FirstOrDefault();
                if (agentProfile != null)
                    nextMileStoneRS.RequiredPointsForTopClub.Top5CountryClub = (agentProfile.TotalPoint - RequestedAgentProfile.TotalPoint) + 1;
            }
            //else
            //{
            //    nextMileStoneRS.RequiredPointsForTopClub.Top5CountryClub=
            //}
            #endregion


            // Get BadgeRule
            conditions = new List<ScanCondition>();
            //conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<BadgeRule> BadgeRules = Dynamo.DynamoDBContext.ScanAsync<BadgeRule>(conditions).GetRemainingAsync().Result;
            BadgeRules = BadgeRules.OrderBy(x => x.RequiredPoint).ToList();

            bool isBadgeMatch = false;
            foreach (BadgeRule BadgeRule in BadgeRules)
            {
                if (isBadgeMatch && agentProfile.BadgeName != BadgeRule.BadgeName)
                {
                    nextMileStoneRS.RequiredPointsForNextBadge = (BadgeRule.RequiredPoint - RequestedAgentProfile.TotalPoint);
                    isBadgeMatch = false;
                }
                if (agentProfile.BadgeName == BadgeRule.BadgeName)
                    isBadgeMatch = true;
            }

            conditions = new List<ScanCondition>();
            // you can add scan conditions, or leave empty
            List<AwardRules> allRules = Dynamo.DynamoDBContext.ScanAsync<AwardRules>(conditions).GetRemainingAsync().Result;
            foreach (AwardRules rules in allRules)
            {
                nextMileStoneRS.NextAwardIdeas.Add("Get " + rules.RulePoint + " % of booking amount for " + rules.RuleName);
            }

            nextMileStoneRS.EarnedPoint = RequestedAgentProfile.TotalPoint;
            nextMileStoneRS.BadgeName = RequestedAgentProfile.BadgeName;

            nextMileStoneRS.StatusCode = StatusCodes.Sussess;
        }
        catch (Exception ex)
        {
            nextMileStoneRS = new NextMileStoneRS { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message };
        }
        return nextMileStoneRS;
    }

    public LeaderboardRankingRS GetLeaderboardRankings(LeaderboardRankingRQ pLeaderboardRankingRQ)
    {
        LeaderboardRankingRS leaderboardRankingRS = new LeaderboardRankingRS();
        try
        {
            var conditions = new List<ScanCondition>();
            // conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<AgentProfile> allAgentProfile = Dynamo.DynamoDBContext.ScanAsync<AgentProfile>(conditions).GetRemainingAsync().Result;

            AgentProfile RequestedAgentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pLeaderboardRankingRQ.AgentCode);

            leaderboardRankingRS.AgentCode = pLeaderboardRankingRQ.AgentCode;

            CalculateRank(allAgentProfile);
            AgentProfile agentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pLeaderboardRankingRQ.AgentCode);
            if (agentProfile != null)
                leaderboardRankingRS.GlobalRank = agentProfile.Rank;

            CalculateRank(allAgentProfile.FindAll(item => item.CityName == agentProfile.CityName));
            agentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pLeaderboardRankingRQ.AgentCode);
            if (agentProfile != null)
                leaderboardRankingRS.CityRank = agentProfile.Rank;

            CalculateRank(allAgentProfile.FindAll(item => item.CountryName == agentProfile.CountryName));
            agentProfile = allAgentProfile.FirstOrDefault(x => x.AgentCode == pLeaderboardRankingRQ.AgentCode);
            if (agentProfile != null)
                leaderboardRankingRS.CountryRank = agentProfile.Rank;


            // Get BadgeRule
            conditions = new List<ScanCondition>();
            //conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAddAwardAfterBookingRQ.AgentCode));
            List<BadgeRule> BadgeRules = Dynamo.DynamoDBContext.ScanAsync<BadgeRule>(conditions).GetRemainingAsync().Result;
            BadgeRules = BadgeRules.OrderBy(x => x.RequiredPoint).ToList();

            bool isBadgeMatch = false;
            foreach (BadgeRule BadgeRule in BadgeRules)
            {
                if (agentProfile.BadgeName == BadgeRule.BadgeName && isBadgeMatch)
                {
                    leaderboardRankingRS.RequiredPointsForNextBadge = (BadgeRule.RequiredPoint - RequestedAgentProfile.TotalPoint);
                    isBadgeMatch = false;
                }
                if (agentProfile.BadgeName == BadgeRule.BadgeName)
                    isBadgeMatch = true;
            }

            leaderboardRankingRS.EarnedPoint = RequestedAgentProfile.TotalPoint;
            leaderboardRankingRS.BadgeName = RequestedAgentProfile.BadgeName;

            leaderboardRankingRS.StatusCode = StatusCodes.Sussess;
        }
        catch (Exception ex)
        {
            leaderboardRankingRS = new LeaderboardRankingRS { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message };
        }
        return leaderboardRankingRS;
    }

    static void CalculateRank(List<AgentProfile> agentProfiles)
    {
        if (agentProfiles != null && agentProfiles.Count > 0)
        {
            // Order the agents by TotalPoint in descending order
            var orderedAgents = agentProfiles.OrderByDescending(agent => agent.TotalPoint).ToList();

            // Assign ranks based on the order
            for (int i = 0; i < orderedAgents.Count; i++)
            {
                orderedAgents[i].Rank = i + 1;
            }
        }
    }
    public AwardHistoryRS GetAwardHistory(AwardHistoryRQ pAwardHistoryRQ)
    {
        AwardHistoryRS awardHistoryRS = new AwardHistoryRS();
        try
        {
            AgentAwardHistory agentAwardHistory = null;

            // Get Award History
            var conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAwardHistoryRQ.AgentCode));
            List<AwardHistory> listAgentAwardHistory = Dynamo.DynamoDBContext.ScanAsync<AwardHistory>(conditions).GetRemainingAsync().Result;

            awardHistoryRS.AgentCode = pAwardHistoryRQ.AgentCode;

            foreach (AwardHistory awardHistory in listAgentAwardHistory)
            {
                agentAwardHistory = new AgentAwardHistory();

                agentAwardHistory.AwardedFor = awardHistory.AgentCode;
                agentAwardHistory.BookingConfirmationNo = awardHistory.BookingConfirmationNo;
                agentAwardHistory.BookingDate = awardHistory.BookingDate;
                agentAwardHistory.EarnedPoint = awardHistory.EarnedPoint;
                agentAwardHistory.BadgeName = awardHistory.BadgeName;
                agentAwardHistory.CityName = awardHistory.CityName;
                agentAwardHistory.CountryCode = awardHistory.CountryCode;
                agentAwardHistory.CountryName = awardHistory.CountryName;
                agentAwardHistory.AwardOn = awardHistory.LastModifyON;
                agentAwardHistory.AwardName = awardHistory.AwardRuleName;
                agentAwardHistory.BookingAmount = awardHistory.BookingAmount;
                awardHistoryRS.AwardHistory.Add(agentAwardHistory);

            }
            awardHistoryRS.StatusCode = StatusCodes.Sussess;
        }
        catch (Exception ex)
        {
            new AwardHistoryRS { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message };
        }
        return awardHistoryRS;
    }


    public ReedimAwardPointsRS ReedimAwardPoints(ReedimAwardPointsRQ pReedimAwardPointsRQ)
    {
        ReedimAwardPointsRS reedimAwardPointsRS = new ReedimAwardPointsRS();
        try
        {
            var tasks = new List<Task>();
            AgentProfile agentProfile = new AgentProfile();

            // Get Agent Profile
            var conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pReedimAwardPointsRQ.AgentCode));
            List<AgentProfile> allAgentProfile = Dynamo.DynamoDBContext.ScanAsync<AgentProfile>(conditions).GetRemainingAsync().Result;
            agentProfile = allAgentProfile[0];

            // Get Redemption History
            conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pReedimAwardPointsRQ.AgentCode));
            List<RedemptionHistory> listRedemptionHistory = Dynamo.DynamoDBContext.ScanAsync<RedemptionHistory>(conditions).GetRemainingAsync().Result;

            RedemptionHistory redemptionHistory = new RedemptionHistory();
            redemptionHistory.AgentCode = pReedimAwardPointsRQ.AgentCode;
            redemptionHistory.RedemptionRuleCode = pReedimAwardPointsRQ.RedemptionRuleCode;
            redemptionHistory.PointsUsed = pReedimAwardPointsRQ.RequirePoints;
            redemptionHistory.LastModifyON = DateTime.Now.ToString();
            redemptionHistory.RedemptionTitle = pReedimAwardPointsRQ.RedemptionTitle;

            tasks.Add(Dynamo.DynamoDBContext.SaveAsync(redemptionHistory));


            agentProfile.CurrentPoint = agentProfile.CurrentPoint - pReedimAwardPointsRQ.RequirePoints;
            tasks.Add(Dynamo.DynamoDBContext.SaveAsync(agentProfile));

            Task.WhenAll(tasks).GetAwaiter().GetResult();
            bool isSucces = true;
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].IsFaulted && tasks[i].Exception != null)
                {
                    isSucces = false;
                }
            }

            if (isSucces)
            {
                reedimAwardPointsRS.AgentCode = pReedimAwardPointsRQ.AgentCode;
                reedimAwardPointsRS.StatusCode = StatusCodes.Sussess;
                reedimAwardPointsRS.CurrentPoint = agentProfile.CurrentPoint;
                reedimAwardPointsRS.TotalPoint = agentProfile.TotalPoint;
                reedimAwardPointsRS.UsedPoint = pReedimAwardPointsRQ.RequirePoints;
            }
            else
            {
                reedimAwardPointsRS.AgentCode = pReedimAwardPointsRQ.AgentCode;
                reedimAwardPointsRS.StatusCode = StatusCodes.Failed;
                reedimAwardPointsRS.ErrorCode = ErrrCodes.PrcessingError;
                reedimAwardPointsRS.Error = "Failed While Updateing ";
            }

        }
        catch (Exception ex)
        {
            reedimAwardPointsRS = new ReedimAwardPointsRS
            {
                StatusCode = StatusCodes.Failed,
                ErrorCode = ErrrCodes.PrcessingError,
                Error = ex.Message
            };
        }
        return reedimAwardPointsRS;
    }

    public List<RedemptionHistory> GetRedemptionHistory(RedemptionHistoryRQ pAwardHistoryRQ)
    {
        List<RedemptionHistory> listRedemptionHistory = new List<RedemptionHistory>();
        try
        {
            // Get Redemption History
            var conditions = new List<ScanCondition>();
            conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAwardHistoryRQ.AgentCode));
            listRedemptionHistory = Dynamo.DynamoDBContext.ScanAsync<RedemptionHistory>(conditions).GetRemainingAsync().Result;

        }
        catch (Exception ex)
        {
            //  listRedemptionHistory.Add(new RedemptionHistory { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message });
        }
        return listRedemptionHistory;
    }

    public List<AgentProfile> GetAgentProfile(AgentProfileRQ pAgentProfileRQ)
    {
        List<AgentProfile> listAgentProfile = new List<AgentProfile>();
        try
        {
            // Get Agent Profile
            var conditions = new List<ScanCondition>();
            if (!string.IsNullOrEmpty(pAgentProfileRQ.AgentCode))
                conditions.Add(new ScanCondition("AgentCode", ScanOperator.Equal, pAgentProfileRQ.AgentCode));
            listAgentProfile = Dynamo.DynamoDBContext.ScanAsync<AgentProfile>(conditions).GetRemainingAsync().Result;

        }
        catch (Exception ex)
        {
            //listRedemptionHistory.Add(new RedemptionHistory { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message });
        }
        return listAgentProfile;
    }

    public List<AgentProfile> GetAgentList()
    {
        List<AgentProfile> listAgentProfile = new List<AgentProfile>();
        try
        {
            // Get Agent Profile
            var conditions = new List<ScanCondition>();
            listAgentProfile = Dynamo.DynamoDBContext.ScanAsync<AgentProfile>(conditions).GetRemainingAsync().Result;

        }
        catch (Exception ex)
        {
            //listRedemptionHistory.Add(new RedemptionHistory { StatusCode = StatusCodes.Failed, ErrorCode = ErrrCodes.PrcessingError, Error = ex.Message });
        }
        return listAgentProfile;
    }
}
