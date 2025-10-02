namespace OrisMall.Core.Configuration;

public class CacheOptions
{
    public const string SectionName = "Cache";
    
    /// <summary>
    /// Maximum number of cache entries. Default is 1000.
    /// </summary>
    public int SizeLimit { get; set; } = 1000;
    
    /// <summary>
    /// Percentage of entries to remove when size limit is reached (0.0 to 1.0). Default is 0.25 (25%).
    /// </summary>
    public double CompactionPercentage { get; set; } = 0.25;
}
