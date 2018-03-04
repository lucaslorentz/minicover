using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MiniCover.Utils
{
    public static class OptimizedStackTrace
    {
        public static Func<Func<MethodBase, bool>, MethodBase> GetTestMethod { get; }

        static OptimizedStackTrace()
        {
            var stackFrameHelperType = typeof(object).Assembly.GetType("System.Diagnostics.StackFrameHelper");
            var variable = Expression.Variable(stackFrameHelperType, "helper");
            var ctor = stackFrameHelperType.GetConstructors()[0];
            var parameter = ctor.GetParameters()[0];
            var helper = Expression.New(ctor, Expression.Constant(null, parameter.ParameterType));

            var getStackFramesInternal = Type.GetType("System.Diagnostics.StackTrace, mscorlib")
                .GetMethod("GetStackFramesInternal", BindingFlags.Static | BindingFlags.NonPublic);
            var getStackFramesInternalExpression = Expression.Call(getStackFramesInternal, variable,
                Expression.Constant(0, typeof(int)), Expression.Constant(false, typeof(Boolean)),
                Expression.Constant(null, typeof(Exception)));

            var getNumberOfFrames = stackFrameHelperType.GetMethod("GetNumberOfFrames");
            var getNumberOfFramesExpression = Expression.Call(variable, getNumberOfFrames);

            var getMethodBase = stackFrameHelperType.GetMethod("GetMethodBase");
            var countVariable = Expression.Variable(typeof(int), "count");
            var methodVariable = Expression.Variable(typeof(MethodBase), "method");

            var loopVar = Expression.Variable(typeof(int), "index");
            var getMethodOfFramesExpression = Expression.Call(variable, getMethodBase, loopVar);

            var paramExpression = Expression.Parameter(typeof(Func<MethodBase, bool>), "condition");

            var returnLabel = Expression.Label("ReturnMethod");
            var block = Expression.Block(
                Expression.Assign(methodVariable, getMethodOfFramesExpression),
                Expression.IfThen(Expression.Invoke(paramExpression, methodVariable),
                    Expression.Return(returnLabel, methodVariable))
            );

            var outsideBlock = Expression.Block(
                new[] { variable, loopVar, countVariable, methodVariable },
                Expression.Assign(variable, helper),
                getStackFramesInternalExpression,
                Expression.Assign(countVariable, Expression.Decrement(getNumberOfFramesExpression)),
                For(loopVar, countVariable, Expression.GreaterThanOrEqual(loopVar, Expression.Constant(0)),
                    Expression.Assign(loopVar, Expression.Decrement(loopVar)), block),
                Expression.Label(returnLabel),
                methodVariable);
            var expression = Expression.Lambda<Func<Func<MethodBase, bool>, MethodBase>>(
                outsideBlock, paramExpression);
            ExpressionTree = expression;
            GetTestMethod = expression.Compile();
        }

        public static Expression<Func<Func<MethodBase, bool>, MethodBase>> ExpressionTree { get;  }

        public static Expression For(ParameterExpression loopVar, Expression initValue, Expression condition, Expression increment, Expression loopContent)
        {
            var initAssign = Expression.Assign(loopVar, initValue);
            var breakLabel = Expression.Label("LoopBreak");
            var loop = Expression.Block(new[] { loopVar },
                initAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        condition,
                        Expression.Block(
                            loopContent,
                            increment
                        ),
                        Expression.Break(breakLabel)
                    ),
                    breakLabel)
            );

            return loop;
        }
    }
}