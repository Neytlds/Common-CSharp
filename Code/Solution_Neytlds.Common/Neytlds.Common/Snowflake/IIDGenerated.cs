using System.Threading.Tasks;

namespace Neytlds.Common.Snowflake
{
    public interface IIDGenerated
    {
        /// <summary>
        ///同步获取新long类型的ID
        /// </summary>
        /// <returns></returns>
        long NextId();

        /// <summary>
        /// 同步获取新string类型的ID
        /// </summary>
        /// <param name="prefix">string类型前缀</param>
        /// <returns></returns>
        string NextStrId(object prefix);

        /// <summary>
        /// 异步获取新long类型的ID
        /// </summary>
        /// <returns></returns>
        Task<long> NextIdAsync();

        /// <summary>
        ///  异步获取新string类型的ID
        /// </summary>
        /// <param name="prefix">string类型前缀</param>
        /// <returns></returns>
        Task<string> NextIdAsync(string prefix);
    }
}
