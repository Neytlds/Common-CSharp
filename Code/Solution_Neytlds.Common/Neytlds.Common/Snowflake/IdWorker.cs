using System;
using System.Threading.Tasks;

namespace Neytlds.Common.Snowflake
{
    public class IdWorker : IIDGenerated
    {
        const int WorkerIdBits = 5;
        const int DatacenterIdBits = 5;
        const int SequenceBits = 12;
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        const long Twepoch = 1454723289800L;
        const int WorkerIdShift = SequenceBits;
        const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        const long SequenceMask = -1L ^ (-1L << SequenceBits);

        long _sequence = 0L;
        long _lastTimestamp = -1L;

        public IdWorker(long workerId, long datacenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            _sequence = sequence;

            // sanity check for workerId
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
            }

        }
        public IdWorker(IWorkerOpation opation)
        {
            WorkerId = opation.GetWorkerId();
            DatacenterId = opation.GetDatacenterId();
        }
        public long WorkerId { get; protected set; }
        public long DatacenterId { get; protected set; }

        readonly object _lock = new object();

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return System.CurrentTimeMillis();
        }


        /// <summary>
        ///同步获取新long类型的ID
        /// </summary>
        /// <returns></returns>
        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                {
                    throw new ArgumentOutOfRangeException("timestamp", $"Clock moved backwards.  Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) |
                         (DatacenterId << DatacenterIdShift) |
                         (WorkerId << WorkerIdShift) | _sequence;

                return id;
            }
        }

        /// <summary>
        /// 同步获取新string类型的ID
        /// </summary>
        /// <param name="prefix">string类型前缀</param>
        /// <returns></returns>
        public virtual string NextStrId(object prefix)
        {
            return $"{prefix}{NextId()}";
        }

        /// <summary>
        /// 异步获取新long类型的ID
        /// </summary>
        /// <returns></returns>
        public Task<long> NextIdAsync()
        {
            return Task.Factory.StartNew(NextId);
        }

        /// <summary>
        ///  异步获取新string类型的ID
        /// </summary>
        /// <param name="prefix">string类型前缀</param>
        /// <returns></returns>
        public Task<string> NextIdAsync(string prefix)
        {
            return Task.Factory.StartNew(NextStrId, prefix);
        }
    }
}
