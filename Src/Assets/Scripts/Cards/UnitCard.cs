namespace GameCard
{
    /// <summary>
    /// 单位卡牌类
    /// </summary>
    public class UnitCard : BaseCard
    {
        /// <summary>
        /// 点击使用卡牌时调用的函数
        /// </summary>
        /// <returns>若成功使用则返回true，中途取消或其他情况返回false</returns>
        public bool Use()
        {
            return true;
        }
    }
}