namespace MassTransit.RedisIntegration.Definition
{
    using System.Text.RegularExpressions;
    using Topology;


    public class SnakeCaseStreamNameFormatter :
        DefaultStreamNameFormatter
    {
        static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled);
        readonly string _separator;

        public SnakeCaseStreamNameFormatter()
        {
            _separator = "_";
        }

        public SnakeCaseStreamNameFormatter(string separator)
        {
            _separator = separator ?? "_";
        }

        public new static IEntityNameFormatter Instance { get; } = new SnakeCaseStreamNameFormatter();

        protected override string SanitizeName(string name)
        {
            return _pattern.Replace(name, m => _separator + m.Value).ToLowerInvariant();
        }
    }
}
