namespace FerOmega.Entities
{
    public class PropertyDef
    {
        public interface IBuilderFrom
        {
            IBuilderTo From(string from);
        }

        public interface IBuilderTo
        {
            PropertyDef ToSql(string to);
        }

        public class BuilderFrom : IBuilderFrom, IBuilderTo
        {
            private string from;

            public IBuilderTo From(string from)
            {
                this.from = from;

                return this;
            }

            public PropertyDef ToSql(string to)
            {
               return new PropertyDef(from, to);
            }
        }

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
