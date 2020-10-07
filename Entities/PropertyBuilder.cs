namespace FerOmega.Entities
{
    public interface IPropertyBuilderFrom
    {
        IPropertyBuilderTo From(string from);
    }

    public interface IPropertyBuilderTo
    {
        PropertyDef ToSql(string to);
    }

    internal class PropertyPropertyPropertyBuilder : IPropertyBuilderFrom, IPropertyBuilderTo
    {
        private string from;

        public IPropertyBuilderTo From(string from)
        {
            this.from = from;

            return this;
        }

        public PropertyDef ToSql(string to)
        {
            return new PropertyDef(from, to);
        }
    }
}