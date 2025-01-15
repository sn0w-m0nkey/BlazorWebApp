namespace Web.Configuration;

public class SendGridConfig
{
    public const string SectionName = "SendGridConfig";
    
    public string? SendGridApiKey { get; set; }
}
