namespace Zhoubin.Infrastructure.Common.Consul.Discover
{
    public class CheckServiceDescrption
    {
        public int? DeregisterCriticalServiceAfter { get; set; }
        public int? Interval { get; set; }
        public int? Timeout { get; set; }
        public string HTTP { get; set; }
    }
}
