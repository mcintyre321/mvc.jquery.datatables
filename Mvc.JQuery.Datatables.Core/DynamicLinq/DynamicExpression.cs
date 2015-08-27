using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mvc.JQuery.Datatables.DynamicLinq
{
    public static class DynamicExpression
    {
        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        public static LambdaExpression ParseLambda(Type itType, string expression, Type baseType = null, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, null, expression, baseType, values);
        }

        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, Type baseType = null, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, baseType, values);
        }

        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, Type baseType = null, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType, baseType), parameters);
        }

        public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
        {
            return (Expression<Func<T, S>>)ParseLambda(typeof(T), typeof(S), expression, null, values);
        }

        public static Type CreateClass(params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(Type resultType = null, Type baseType = null, params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties, resultType, baseType);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties, Type resultType = null, Type baseType = null)
        {
            return ClassFactory.Instance.GetDynamicClass(properties, resultType, baseType);
        }
    }
}