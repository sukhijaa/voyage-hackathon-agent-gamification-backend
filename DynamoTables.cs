
using Amazon.DynamoDBv2.DataModel;
using System.Xml.Serialization;
namespace dynamoDb.model
{
    [DynamoDBTable("AgentProfile")]
    public class AgentProfile
    {
        [DynamoDBHashKey("AgentCode")]
        public string AgentCode { get; set; }

        [DynamoDBProperty("AgentName")]
        public string AgentName { get; set; }

        [DynamoDBProperty("TotalPoint")]
        public int TotalPoint { get; set; }

        [DynamoDBRangeKey("AgencyName")]
        public string AgencyName { get; set; }

        [DynamoDBProperty("BadgeName")]
        public string BadgeName { get; set; }

        [DynamoDBProperty("CityName")]
        public string CityName { get; set; }

        [DynamoDBProperty("CountryCode")]
        public string CountryCode { get; set; }

        [DynamoDBProperty("CountryName")]
        public string CountryName { get; set; }

        [DynamoDBProperty("CurrentPoint")]
        public int CurrentPoint { get; set; }

        [DynamoDBProperty("LastModifyON")]
        public string LastModifyON { get; set; }

        [DynamoDBProperty("Rank")]
        public int Rank { get; set; }

        [DynamoDBProperty("LastLoginTime")]
        public string LastLoginTime { get; set; }

    }

    [DynamoDBTable("AwardRules")]
    public class AwardRules
    {
        [DynamoDBHashKey("RuleCode")]
        public string RuleCode { get; set; }

        [DynamoDBProperty("RuleName")]
        public string RuleName { get; set; }

        [DynamoDBRangeKey("RuleType")]
        public string RuleType { get; set; }

        [DynamoDBProperty("RulePoint")]
        public int RulePoint { get; set; }

        [DynamoDBProperty("LastModifyON")]
        public string LastModifyON { get; set; }
    }

    [DynamoDBTable("AwardHistory")]
    public class AwardHistory
    {
        [DynamoDBHashKey("AgentCode_AwardRuleCode_BookingConfirmationNo")]
        public string AgentCode_AwardRuleCode_BookingConfirmationNo { get; set; }

        [DynamoDBRangeKey("AgentCode")]
        public string AgentCode { get; set; }

        [DynamoDBProperty("AwardRuleCode")]
        public string AwardRuleCode { get; set; }

        [DynamoDBProperty("AwardRuleName")]
        public string AwardRuleName { get; set; }

        [DynamoDBProperty("BookingConfirmationNo")]
        public string BookingConfirmationNo { get; set; }

        [DynamoDBProperty("BookingDate")]
        public string BookingDate { get; set; }

        [DynamoDBProperty("BookingAmount")]
        public int BookingAmount { get; set; }

        [DynamoDBProperty("EarnedPoint")]
        public int EarnedPoint { get; set; }

        [DynamoDBProperty("BadgeName")]
        public string BadgeName { get; set; }

        [DynamoDBProperty("CityName")]
        public string CityName { get; set; }

        [DynamoDBProperty("CountryName")]
        public string CountryName { get; set; }

        [DynamoDBProperty("CountryCode")]
        public string CountryCode { get; set; }

        [DynamoDBProperty("LastModifyON")]
        public string LastModifyON { get; set; }
    }

    [DynamoDBTable("RedemptionRule")]
    public class RedemptionRule
    {
        [DynamoDBHashKey("RuleCode")]
        public string RuleCode { get; set; }

        [DynamoDBRangeKey("RuleName")]
        public string RuleName { get; set; }

        [DynamoDBProperty("RequiredPoint")]
        public string RequiredPoint { get; set; }

        [DynamoDBProperty("LastModifyON")]
        public string LastModifyON { get; set; }
    }

    [DynamoDBTable("BadgeRule")]
    public class BadgeRule
    {
        [DynamoDBHashKey("BadgeCode")]
        public string BadgeCode { get; set; }

        [DynamoDBRangeKey("BadgeName")]
        public string BadgeName { get; set; }

        [DynamoDBProperty("RequiredPoint")]
        public int RequiredPoint { get; set; }

        [DynamoDBProperty("LastModifyON")]
        public string LastModifyON { get; set; }
    }

    [DynamoDBTable("RedemptionHistory")]
    public class RedemptionHistory
    {

        [DynamoDBRangeKey("AgentCode")]
        public string AgentCode { get; set; }

        [DynamoDBHashKey("RedemptionRuleCode")]
        public string RedemptionRuleCode { get; set; }

        [DynamoDBProperty("PointsUsed")]
        public int PointsUsed { get; set; }

        [DynamoDBProperty("LastModifyON")]
        public string LastModifyON { get; set; }

        [DynamoDBProperty("RedemptionTitle")]
        public string RedemptionTitle { get; set; }

        //[DynamoDBProperty("LastModifyON")]
        //public StatusCodes StatusCode { get; set; }

        //[DynamoDBProperty("LastModifyON")]
        //public ErrrCodes ErrorCode { get; set; }

        //[DynamoDBProperty("LastModifyON")]
        //public string Error { get; set; }
    }
}


