using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FerOmega.Entities;

namespace FerOmega.Services
{
    internal static class Extensions
    {
        private static readonly OperatorType[] openBrackets = 
        {
            OperatorType.OpenCurlyBracket,
            OperatorType.OpenRoundBracket,
            OperatorType.OpenSquareBracket,
        };

        private static readonly OperatorType[] closeBrackets =
        {
            OperatorType.CloseCurlyBracket,
            OperatorType.CloseRoundBracket,
            OperatorType.CloseSquareBracket,
        };

        public static bool IsOpenBracket(this OperatorType operatorType)
        {
            return openBrackets.Contains(operatorType);
        }

        public static bool IsCloseBracket(this OperatorType operatorType)
        {
            return closeBrackets.Contains(operatorType);
        }
        public static bool IsBracket(this OperatorType operatorType)
        {
            return IsOpenBracket(operatorType) || IsCloseBracket(operatorType);
        }

        public static bool IsOpenBracket(this AbstractToken @operator)
        {
            return IsOpenBracket(@operator.OperatorType);
        }

        public static bool IsCloseBracket(this AbstractToken @operator)
        {
            return IsCloseBracket(@operator.OperatorType);
        }

        public static bool IsBracket(this AbstractToken @operator)
        {
            return IsOpenBracket(@operator) || IsCloseBracket(@operator);
        }
    }
}
