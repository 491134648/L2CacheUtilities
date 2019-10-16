namespace FH.Cache.Core.Stats
{
    /// <summary>
    /// 
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// name
        /// </summary>
        string Name { get; }
        string Reason { get; }
        bool IsFinal { get; }
    }
}
