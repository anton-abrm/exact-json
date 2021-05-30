using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExactJson.Infra
{
    internal static class ReflectionUtil
    {
        public static bool IsNullAssignable(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            return !type.IsValueType || IsNullable(type);
        }

        public static bool IsNullable(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            return Nullable.GetUnderlyingType(type) != null;
        }

        public static Type UnwrapNullable(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            for (var baseType = type.BaseType; baseType != null; baseType = baseType.BaseType) {
                yield return baseType;
            }
        }

        public static Func<TTarget> CreateDefaultConstructor<TTarget>(Type type)
        {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            var castedToObjectNewExpr = Expression.Convert(Expression.New(type), typeof(TTarget));

            return Expression.Lambda<Func<TTarget>>(castedToObjectNewExpr).Compile();
        }

        public static Func<TTarget> CreateDefaultConstructor<TTarget>(ConstructorInfo constructorInfo)
        {
            if (constructorInfo is null) {
                throw new ArgumentNullException(nameof(constructorInfo));
            }

            if (constructorInfo.GetParameters().Length != 0) {
                throw new ArgumentException("Constructor must not have any parameters.", nameof(constructorInfo));
            }

            var castedToObjectPropertyGetterExpr = Expression.Convert(Expression.New(constructorInfo), typeof(TTarget));

            return Expression.Lambda<Func<TTarget>>(castedToObjectPropertyGetterExpr).Compile();
        }

        public static Func<TTarget, TValue> CreatePropertyGetter<TTarget, TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null) {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var objectTargetParamExpr = Expression.Parameter(typeof(TTarget));

            var castedToTypeTargetParamExpr = Expression.Convert(objectTargetParamExpr, propertyInfo.DeclaringType);

            var propertyAccessExpr = Expression.Property(castedToTypeTargetParamExpr, propertyInfo);

            var castedToTPropertyGetterExpr = Expression.Convert(propertyAccessExpr, typeof(TValue));

            return Expression.Lambda<Func<TTarget, TValue>>(castedToTPropertyGetterExpr, objectTargetParamExpr)
                             .Compile();
        }
        
        public static Func<TTarget, TValue, TTarget> CreatePropertySetter<TTarget, TValue>(PropertyInfo propertyInfo)
        {
            if (propertyInfo is null) {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (!propertyInfo.CanWrite) {
                throw new ArgumentException("Property should be writable.", nameof(propertyInfo));
            }

            var target = Expression.Parameter(typeof(TTarget));
            var value = Expression.Parameter(typeof(TValue));

            var castedTarget = Expression.Variable(propertyInfo.DeclaringType);

            var block = Expression.Block(
                typeof(TTarget),
                new[] { castedTarget },
                Expression.Assign(castedTarget, 
                    Expression.Convert(target, propertyInfo.DeclaringType)),
                Expression.Assign(Expression.Property(castedTarget, propertyInfo),
                    Expression.Convert(value, propertyInfo.PropertyType)),
                Expression.Convert(castedTarget, typeof(TTarget)));

            return Expression.Lambda<Func<TTarget, TValue, TTarget>>(block, target, value)
                             .Compile();
        }

        public static Action<TTarget> CreateActionMethodInvoker<TTarget>(MethodInfo methodInfo)
        {
            if (methodInfo is null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 0) {
                throw new ArgumentException("Method must not have any parameters.", nameof(methodInfo));
            }

            var objectTargetParamExpr = Expression.Parameter(typeof(TTarget));
            var castedToTypeTargetParamExpr = Expression.Convert(objectTargetParamExpr, methodInfo.DeclaringType);

            var callExpr = Expression.Call(castedToTypeTargetParamExpr, methodInfo);

            return Expression.Lambda<Action<TTarget>>(callExpr, objectTargetParamExpr).Compile();
        }

        public static Action<TTarget, TArg> CreateActionMethodInvoker<TTarget, TArg>(MethodInfo methodInfo)
        {
            if (methodInfo is null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) {
                throw new ArgumentException("Method must have 1 parameter.", nameof(methodInfo));
            }

            var objectTargetParamExpr = Expression.Parameter(typeof(TTarget));
            var objectValueParamExpr = Expression.Parameter(typeof(TArg));

            var castedToTypeTargetParamExpr = Expression.Convert(objectTargetParamExpr, methodInfo.DeclaringType);
            var castedToTypeValueParamExpr = Expression.Convert(objectValueParamExpr, parameters[0].ParameterType);

            var callExpr = Expression.Call(castedToTypeTargetParamExpr, methodInfo, castedToTypeValueParamExpr);

            return Expression.Lambda<Action<TTarget, TArg>>(callExpr, objectTargetParamExpr, objectValueParamExpr)
                             .Compile();
        }

        public static Action<TTarget, TArg1, TArg2> CreateActionMethodInvoker<TTarget, TArg1, TArg2>(MethodInfo methodInfo)
        {
            if (methodInfo is null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 2) {
                throw new ArgumentException("Method must have 2 parameter.", nameof(methodInfo));
            }

            var objectTargetParamExpr = Expression.Parameter(typeof(TTarget));
            var objectValueParam1Expr = Expression.Parameter(typeof(TArg1));
            var objectValueParam2Expr = Expression.Parameter(typeof(TArg2));

            var castedToTypeTargetParamExpr = Expression.Convert(objectTargetParamExpr, methodInfo.DeclaringType);
            var castedToTypeValueParam1Expr = Expression.Convert(objectValueParam1Expr, parameters[0].ParameterType);
            var castedToTypeValueParam2Expr = Expression.Convert(objectValueParam2Expr, parameters[1].ParameterType);

            var callExpr = Expression.Call(
                castedToTypeTargetParamExpr,
                methodInfo,
                castedToTypeValueParam1Expr,
                castedToTypeValueParam2Expr);

            return Expression
                  .Lambda<Action<TTarget, TArg1, TArg2>>(callExpr, objectTargetParamExpr, objectValueParam1Expr, objectValueParam2Expr)
                  .Compile();
        }

        public static Func<TTarget, TResult> CreateFuncMethodInvoker<TTarget, TResult>(MethodInfo methodInfo)
        {
            if (methodInfo is null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            var objectTargetParamExpr = Expression.Parameter(typeof(TTarget));

            var castedToTypeTargetParamExpr = Expression.Convert(objectTargetParamExpr, methodInfo.DeclaringType);

            var callExpression = Expression.Call(castedToTypeTargetParamExpr, methodInfo);

            var castedToResultExpr = Expression.Convert(callExpression, typeof(TResult));

            return Expression.Lambda<Func<TTarget, TResult>>(castedToResultExpr, objectTargetParamExpr)
                             .Compile();
        }
    }
}