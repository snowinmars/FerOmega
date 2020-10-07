namespace FerOmega.Entities
{
    public class PropertyDef
    {
        /// <summary>
        /// It's possible to use <c>PropertyBuilder</c>
        /// </summary>
        public PropertyDef(string from, string to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Ui property
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Sql column
        /// </summary>
        public string To { get; }

        public override string ToString()
        {
            return $"{From} > {To}";
        }
    }
}