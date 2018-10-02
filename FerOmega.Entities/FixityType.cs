using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerOmega.Entities
{
    public enum FixityType
    {
        /// <summary>
        /// operand OPERATOR operand
        /// </summary>
        Infix,

        /// <summary>
        /// OPERATOR operand
        /// </summary>
        Prefix,

        /// <summary>
        /// operand OPERATOR
        /// </summary>
        Postfix,

        /// <summary>
        /// OPERATOR operand OPERATOR
        /// </summary>
        Circumflex,

        /// <summary>
        /// operand OPERATOR operand OPERATOR
        /// </summary>
        PostCircumflex
    }
}
