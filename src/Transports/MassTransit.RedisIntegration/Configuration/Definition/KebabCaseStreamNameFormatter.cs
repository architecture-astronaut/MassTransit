namespace MassTransit.RedisIntegration.Definition
{
    using Topology;


    /// <summary>
    /// Formats the Stream names using kebab-case (dashed snake case)
    /// SubmitOrder-> submit-order
    /// OrderState -> order-state
    /// </summary>
    public class KebabCaseStreamNameFormatter :
        SnakeCaseStreamNameFormatter
    {
        public KebabCaseStreamNameFormatter()
            : base("-")
        {
        }

        public new static IEntityNameFormatter Instance { get; } = new KebabCaseStreamNameFormatter();

        protected override string SanitizeName(string name)
        {
            return base.SanitizeName(name).Replace('_', '-');
        }
    }
}
