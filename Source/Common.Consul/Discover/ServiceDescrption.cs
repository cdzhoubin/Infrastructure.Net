namespace Zhoubin.Infrastructure.Common.Consul.Discover
{
    public class ServiceDescrption
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        public CheckServiceDescrption CheckSerivce { get; set; }
    }
}
