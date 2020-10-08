using System;
using System.Text;

namespace FerOmega.Entities
{
    public class SqlFilter
    {
        private readonly string where;

        private bool HasCount => count != null;

        private bool HasPage => page != null;
        
        private bool HasWhere => !string.IsNullOrWhiteSpace(where);

        private int? count;
        private int? page;

        public SqlFilter(string where)
        {
            this.where = where;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (HasPage && !HasCount)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Cannot calculate sql with 'page' but without 'count per page'");
            }
            
            if (HasWhere)
            {
                sb.Append($"where {where}");
            }

            if (HasCount)
            {
                sb.Append($" limit {count}");
            }

            if (HasPage)
            {
                sb.Append($" offset {count * page}");
            }
            
            return sb.ToString();
        }

        public SqlFilter AddCount(int count)
        {
            if (HasCount)
            {
                throw new InvalidOperationException("Already has count");
            }

            this.count = count;

            return this;
        }
        
        public SqlFilter AddPage(int page)
        {
            if (HasPage)
            {
                throw new InvalidOperationException("Already has page");
            }

            this.page = page;

            return this;
        }
    }
}