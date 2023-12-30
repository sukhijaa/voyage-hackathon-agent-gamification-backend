using System;


public enum StatusCodes
{
    Sussess = 200,
    Failed = 400,
    ServerError = 500
}

public enum ErrrCodes
{
    BadRequest = 400,
    NotFound = 404,
    ServerError = 500,
    BadGateway = 502,
    ServiceUnavailable = 503,
    PrcessingError = 505,
}

public class AddAwardAfterBookingRQ
{
    public AddAwardAfterBookingRQ()
    {
    }

    public string AgentCode { get; set; }
    public string BookingConfirmationNo { get; set; }
    public string CityName { get; set; }
    public string CountryName { get; set; }
    public int BookingAmount { get; set; }
    public DateTime BookingDate { get; set; }

    public string CountryCode { get; set; }

}
public class AddAwardAfterBookingRS
{
    public AddAwardAfterBookingRS()
    {
    }

    public string AgentCode { get; set; }
    public StatusCodes StatusCode { get; set; }
    public ErrrCodes ErrorCode { get; set; }
    public string Error { get; set; }
    public int EarnedPoint { get; set; }
    public int TotalPoint { get; set; }
    public string UpgradedBadgeName { get; set; }

}

public class NextMileStoneRQ
{
    public NextMileStoneRQ()
    { }
    public string AgentCode { get; set; }
}

public class NextMileStoneRS
{
    public NextMileStoneRS()
    { }
    public string AgentCode { get; set; }
    public StatusCodes StatusCode { get; set; }
    public ErrrCodes ErrorCode { get; set; }
    public string Error { get; set; }
    public int EarnedPoint { get; set; }
    public string BadgeName { get; set; }
    public int GlobalRank { get; set; }
    public int CityRank { get; set; }
    public int CountryRank { get; set; }
    public int RequiredPointsForNextBadge { get; set; }
    public RequiredPointsForTopClub RequiredPointsForTopClub { get; set; }

    public List<string> NextAwardIdeas { get; set; } = new List<string>();
}

public class RequiredPointsForTopClub
{
    public int Top5GlobalClub { get; set; }
    public int Top5CountryClub { get; set; }
    public int Top5CityCub { get; set; }
}

public class LeaderboardRankingRQ
{
    public LeaderboardRankingRQ()
    { }
    public string AgentCode { get; set; }
}

public class LeaderboardRankingRS
{
    public LeaderboardRankingRS()
    { }
    public string AgentCode { get; set; }
    public StatusCodes StatusCode { get; set; }
    public ErrrCodes ErrorCode { get; set; }
    public string Error { get; set; }
    public int EarnedPoint { get; set; }
    public string BadgeName { get; set; }
    public int GlobalRank { get; set; }
    public int CityRank { get; set; }
    public int CountryRank { get; set; }
    public int RequiredPointsForNextBadge { get; set; }
}

public class AwardHistoryRQ
{
    public AwardHistoryRQ()
    { }
    public string AgentCode { get; set; }
}


public class AwardHistoryRS
{
    public AwardHistoryRS()
    { }
    public string AgentCode { get; set; }
    public StatusCodes StatusCode { get; set; }
    public ErrrCodes ErrorCode { get; set; }
    public string Error { get; set; }
    public List<AgentAwardHistory> AwardHistory { get; set; } = new List<AgentAwardHistory>();
}

public class AgentAwardHistory
{
    public string AwardedFor { get; set; }
    public string BookingConfirmationNo { get; set; }
    public string BookingDate { get; set; }
    public int EarnedPoint { get; set; }
    public string BadgeName { get; set; }
    public string CityName { get; set; }
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public string AwardOn { get; set; }

    public string AwardName { get; set; }

    public int BookingAmount { get; set; }

}



public class Rule
{
    public string RuleCode { get; set; }
    public string RuleType { get; set; }
    public string RuleName { get; set; }
}

public class ReedimAwardPointsRQ
{
    public string AgentCode { get; set; }
    public string RedemptionRuleCode { get; set; }
    public int RequirePoints { get; set; }
    public string RedemptionTitle { get; set; }
}

public class ReedimAwardPointsRS
{
    public string AgentCode { get; set; }
    public StatusCodes StatusCode { get; set; }
    public ErrrCodes ErrorCode { get; set; }
    public string Error { get; set; }
    public int CurrentPoint { get; set; }
    public int TotalPoint { get; set; }
    public int UsedPoint { get; set; }
}

public class RedemptionHistoryRQ
{
    public string AgentCode { get; set; }
}


public class AgentProfileRQ
{
    public string AgentCode { get; set; }
}