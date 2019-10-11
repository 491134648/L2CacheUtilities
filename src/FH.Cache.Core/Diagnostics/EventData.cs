namespace FH.Cache.Core.Diagnostics
{
    /// <summary>
    /// 事件数据
    /// </summary>
    public class EventData
    {
        /// <summary>
        /// 初始化事件数据
        /// </summary>
        /// <param name="cacheType">缓存类型</param>
        /// <param name="name">名称</param>
        /// <param name="operation">操作</param>
        public EventData(string cacheType, string name, string operation)
        {
            this.CacheType = cacheType;
            this.Name = name;
            this.Operation = operation;
        }
        /// <summary>
        /// 缓存类型
        /// </summary>
        public string CacheType { get; set; }
        /// <summary>
        /// 缓存名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }
    }
}
