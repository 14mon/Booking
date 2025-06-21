using StackExchange.Redis;

namespace booking_system.Redis
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _readConnection;
        private readonly ConnectionMultiplexer _writeConnection;

        public RedisService(string readConnection, string writeConnection, string instanceName)
        {
            var readOptions = ConfigurationOptions.Parse(readConnection);
            readOptions.AbortOnConnectFail = false;
            readOptions.ClientName = instanceName;
            _readConnection = ConnectionMultiplexer.Connect(readOptions);

            var writeOptions = ConfigurationOptions.Parse(writeConnection);
            writeOptions.AbortOnConnectFail = false;
            writeOptions.ClientName = instanceName;
            _writeConnection = ConnectionMultiplexer.Connect(writeOptions);
        }

        public IDatabase GetRedisReadDatabase(int db = 0) => _readConnection.GetDatabase(db);

        public IDatabase GetRedisWriteDatabase(int db = 0) => _writeConnection.GetDatabase(db);

        public void Close()
        {
            if (_readConnection.IsConnected)
                _readConnection.Dispose();

            if (_writeConnection.IsConnected)
                _writeConnection.Dispose();
        }
    }
}
