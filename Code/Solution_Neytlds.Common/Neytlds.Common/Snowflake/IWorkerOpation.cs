namespace Neytlds.Common.Snowflake
{
    public interface IWorkerOpation
    {
        /// <summary>
        /// 最大值500
        /// </summary>
        /// <returns></returns>
        long GetWorkerId();

        /// <summary>
        /// 最大值500
        /// </summary>
        /// <returns></returns>
        long GetDatacenterId();
    }
}
