﻿using System.Reflection;
using Xunit;
using Assert = Xunit.Assert;
// ReSharper disable RedundantEnumerableCastCall
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertToConstant.Local
// ReSharper disable RedundantLogicalConditionalExpressionOperand
// ReSharper disable RedundantCast
// ReSharper disable ConstantNullCoalescingCondition
// ReSharper disable EqualExpressionComparison
// ReSharper disable RedundantToStringCall
#pragma warning disable 1720
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using ExpressionToCodeLib;

namespace ExpressionToCodeTest
{
    public class ExpressionToCodeLibTest
    {
        [Fact]
        public void ComplexObjectToCodeAlsoSupportsExpressions()
        {
            Assert.Equal("() => 42", ObjectToCode.ComplexObjectToPseudoCode((Expression<Func<int>>)(() => 42)));
        }

        [Fact]
        public void AddOperator()
        {
            var x = 0;
            Assert.Equal(
                @"() => 1 + x + 2 == 4",
                ExpressionToCode.ToCode(() => 1 + x + 2 == 4));
        }

        [Fact]
        public void AnonymousClasses()
        {
            Assert.Equal(
                @"() => new { X = 3, A = ""a"" } == new { X = 3, A = ""a"" }",
                ExpressionToCode.ToCode(() => new { X = 3, A = "a" } == new { X = 3, A = "a" }));
        }

        [Fact]
        public void ArrayIndex()
        {
            Assert.Equal(
                @"() => new[] { 3, 4, 5 }[0 + (int)(DateTime.Now.Ticks % 3L)] == 3",
                ExpressionToCode.ToCode(() => new[] { 3, 4, 5 }[0 + (int)(DateTime.Now.Ticks % 3)] == 3));
        }

        [Fact]
        public void ArrayLengthAndDoubles()
        {
            Assert.Equal(
                @"() => new[] { 1.0, 2.01, 3.5 }.Concat(new[] { 1.0, 2.0 }).ToArray().Length == 0",
                ExpressionToCode.ToCode(() => new[] { 1.0, 2.01, 3.5 }.Concat(new[] { 1.0, 2.0 }).ToArray().Length == 0));
        }

        [Fact]
        public void AsOperator()
        {
            Assert.Equal(
                @"() => new object() as string == default(string)",
                ExpressionToCode.ToCode(() => new object() as string == null));
        }

        [Fact]
        public void ComplexGenericName()
        {
            Assert.Equal(
                @"() => ((Func<int, bool>)(x => x > 0))(0)",
                ExpressionToCode.ToCode(() => ((Func<int, bool>)(x => x > 0))(0)));
        }

        [Fact]
        public void DefaultValue()
        {
            Assert.Equal(
                @"() => new TimeSpan(1, 2, 3) == default(TimeSpan)",
                ExpressionToCode.ToCode(() => new TimeSpan(1, 2, 3) == default(TimeSpan)));
        }

        [Fact]
        public void IndexerAccess()
        {
            var dict = Enumerable.Range(1, 20).ToDictionary(n => n.ToString());
            Assert.Equal(
                @"() => dict[""3""] == 3",
                ExpressionToCode.ToCode(() => dict["3"] == 3));
        }

        [Fact]
        public void IsOperator()
        {
            Assert.Equal(
                @"() => new object() is string",
                ExpressionToCode.ToCode(() => new object() is string));
        }

        [Fact]
        public void ArrayOfFuncInitializer()
        {
            Assert.Equal(
                @"() => new Func<int>[] { () => 1, () => 2 }",
                ExpressionToCode.ToCode(() => new Func<int>[] { () => 1, () => 2 }));
        }

        [Fact]
        public void ArrayOfFuncInitializer_FullNames()
        {
            Assert.Equal(
                @"() => new System.Func<int>[] { () => 1, () => 2 }",
                ExpressionToCodeConfiguration.DefaultCodeGenConfiguration.WithObjectStringifier(ObjectStringify.WithFullTypeNames)
                    .GetExpressionToCode()
                    .ToCode(() => new Func<int>[] { () => 1, () => 2 }));
        }

        [Fact]
        public void ListInitializer()
        {
            Assert.Equal(
                @"() => new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 4 } }.Count == 3",
                ExpressionToCode.ToCode(() => new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 4 } }.Count == 3));
        }

        [Fact]
        public void ListInitializer2()
        {
            Assert.Equal(
                @"() => new List<int>(50) { 1, 2, 3 }.Count == 3",
                ExpressionToCode.ToCode(() => new List<int>(50) { 1, 2, 3 }.Count == 3));
        }

        [Fact]
        public void ListInitializer3()
        {
            Assert.Equal(
                @"() => new List<int> { 1, 2, 3 }.Count == 3",
                ExpressionToCode.ToCode(() => new List<int> { 1, 2, 3 }.Count == 3));
        }

        [Fact]
        public void LiteralCharAndProperty()
        {
            Assert.Equal(
                @"() => new string(' ', 3).Length == 1",
                ExpressionToCode.ToCode(() => new string(' ', 3).Length == 1));
        }

        [Fact]
        public void MembersBuiltin()
        {
            Assert.Equal(
                @"() => 1.23m.ToString()",
                ExpressionToCode.ToCode(() => 1.23m.ToString()));
            Assert.Equal(
                @"() => AttributeTargets.All.HasFlag((Enum)AttributeTargets.Assembly)",
                ExpressionToCode.ToCode(() => AttributeTargets.All.HasFlag((Enum)AttributeTargets.Assembly)));
            Assert.Equal(
                @"() => ""abc"".Length == 3",
                ExpressionToCode.ToCode(() => "abc".Length == 3));
            Assert.Equal(
                @"() => 'a'.CompareTo('b') < 0",
                ExpressionToCode.ToCode(() => 'a'.CompareTo('b') < 0));
        }

        [Fact]
        public void MembersDefault()
        {
            Assert.Equal(
                @"() => default(DateTime).Ticks == 0L",
                ExpressionToCode.ToCode(() => default(DateTime).Ticks == 0L));
            Assert.Equal(
                @"() => default(int[]).Length == 0",
                ExpressionToCode.ToCode(() => default(int[]).Length == 0));
            Assert.Equal(
                @"() => default(Type).GetTypeInfo().IsLayoutSequential",
                ExpressionToCode.ToCode(() => default(Type).GetTypeInfo().IsLayoutSequential));
            Assert.Equal(
                @"() => default(List<int>).Count",
                ExpressionToCode.ToCode(() => default(List<int>).Count));
            Assert.Equal(
                @"() => default(int[]).Clone() == null",
                ExpressionToCode.ToCode(() => default(int[]).Clone() == null));
            //default(Type).IsInstanceOfType(new object()) is not compiled as extension method in .net core!
            Assert.Equal(
                @"() => default(IEnumerable<Type>).Any()",
                ExpressionToCode.ToCode(() => default(IEnumerable<Type>).Any()));
            Assert.Equal(
                @"() => default(List<int>).AsReadOnly()",
                ExpressionToCode.ToCode(() => default(List<int>).AsReadOnly()));
        }

        [Fact]
        public void MembersThis()
        {
            new ClassA().DoAssert();
        }

        [Fact]
        public void MethodGroupAsExtensionMethod()
        {
            var actual = ExpressionToCode.ToCode(() => (Func<bool>)new[] { 2000, 2004, 2008, 2012 }.Any);
            Assert.Equal(
                "() => (Func<bool>)new[] { 2000, 2004, 2008, 2012 }.Any",
                actual);
            Console.WriteLine(actual);
        }

        [Fact]
        public void MethodGroupConstant()
        {
            Assert.Equal(
                @"() => Array.TrueForAll(new[] { 2000, 2004, 2008, 2012 }, (Predicate<int>)DateTime.IsLeapYear)",
                ExpressionToCode.ToCode(() => Array.TrueForAll(new[] { 2000, 2004, 2008, 2012 }, DateTime.IsLeapYear)));

            var set = new HashSet<int>();
            Assert.Equal(
                @"() => new[] { 2000, 2004, 2008, 2012 }.All((Func<int, bool>)set.Add)",
                ExpressionToCode.ToCode(() => new[] { 2000, 2004, 2008, 2012 }.All(set.Add)));

            Func<Func<object, object, bool>, bool> sink = f => f(null, null);
            Assert.Equal(
                @"() => sink((Func<object, object, bool>)object.Equals)",
                ExpressionToCode.ToCode(() => sink(int.Equals)));
        }

        [Fact]
        public void MultipleCasts()
        {
            Assert.Equal(
                @"() => 1 == (int)(object)1",
                ExpressionToCode.ToCode(() => 1 == (int)(object)1));
        }

        [Fact]
        public void MultipleDots()
        {
            Assert.Equal(
                @"() => 3.ToString().ToString().Length > 0",
                ExpressionToCode.ToCode(() => 3.ToString().ToString().Length > 0));
        }

        [Fact]
        public void NestedLambda_NoParameters()
        {
            Func<Func<int>, int> call = f => f();
            Assert.Equal(
                @"() => call(() => 42)",
                ExpressionToCode.ToCode(() => call(() => 42))
            ); //no params
        }

        [Fact]
        public void NestedLambda_OneParameter()
        {
            Assert.Equal(
                @"() => new[] { 37, 42 }.Select(x => x * 2)",
                ExpressionToCode.ToCode(() => new[] { 37, 42 }.Select(x => x * 2))
            );
            Assert.Equal(
                @"() => Buzz(x => true)",
                ExpressionToCode.ToCode(() => Buzz(x => true))
            );
        }

        [Fact]
        public void NestedLambda_TwoParameters()
        {
            Assert.Equal(
                @"() => new[] { 37, 42 }.Select((x, i) => x * 2)",
                ExpressionToCode.ToCode(() => new[] { 37, 42 }.Select((x, i) => x * 2))
            );
        }

        bool Fizz(Func<int, bool> a) => a(42);

        bool Buzz(Func<int, bool> a) => a(42);

        bool Fizz(Func<string, bool> a) => a("42");

        [Fact]
        public void NestedLambda_MultipleOverloads()
        {
            Assert.Equal(
                @"() => Fizz(x => x == ""a"")",
                ExpressionToCode.ToCode(() => Fizz(x => x == "a"))
            );
            Assert.Equal(
                @"() => Fizz(x => x == 37)",
                ExpressionToCode.ToCode(() => Fizz(x => x == 37))
            );
        }

        [Fact(Skip = "issue 14")]
        public void NestedLambda_UncertainOverload()
        {
            Assert.Equal(
                @"() => Fizz((int x) => true)",
                ExpressionToCode.ToCode(() => Fizz((int x) => true))
            ); //hard case!
        }

        [Fact]
        public void NewArrayAndExtensionMethod()
        {
            Assert.Equal(
                @"() => new[] { 1.0, 2.01, 3.5 }.SequenceEqual(new[] { 1.0, 2.01, 3.5 })",
                ExpressionToCode.ToCode(() => new[] { 1.0, 2.01, 3.5 }.SequenceEqual(new[] { 1.0, 2.01, 3.5 })));
        }

        [Fact]
        public void NewMultiDimArray()
        {
            Assert.Equal(
                @"() => new int[3, 4].Length == 1",
                ExpressionToCode.ToCode(() => new int[3, 4].Length == 1));
        }

        [Fact]
        public void NewObject()
        {
            Assert.Equal(
                @"() => new object()",
                ExpressionToCode.ToCode(() => new object()));
        }

        [Fact]
        public void NewObjectNotEqualsNewObject()
        {
            Assert.Equal(
                @"() => new object() != new object()",
                ExpressionToCode.ToCode(() => new object() != new object()));
        }

        [Fact]
        public void NotOperator()
        {
            var x = true;
            var y = 3;
            byte z = 42;
            Assert.Equal(
                @"() => ~(int)z == 0",
                ExpressionToCode.ToCode(() => ~(int)z == 0));
            Assert.Equal(
                @"() => ~y == 0",
                ExpressionToCode.ToCode(() => ~y == 0));
            Assert.Equal(
                @"() => !x",
                ExpressionToCode.ToCode(() => !x));
        }

        [Fact]
        public void ObjectInitializers()
        {
            var s = new XmlReaderSettings {
                CloseInput = false,
                CheckCharacters = false
            };
            Assert.Equal(
                @"() => new XmlReaderSettings { CloseInput = s.CloseInput, CheckCharacters = s.CheckCharacters }.Equals(s)",
                ExpressionToCode.ToCode(
                    () => new XmlReaderSettings { CloseInput = s.CloseInput, CheckCharacters = s.CheckCharacters }.Equals(s)));
        }

        [Fact]
        public void Quoted()
        {
            Assert.Equal(
                @"() => (Expression<Func<int, string, string>>)((n, s) => s + n.ToString()) != null",
                ExpressionToCode.ToCode(() => (Expression<Func<int, string, string>>)((n, s) => s + n.ToString()) != null));
        }

        [Fact]
        public void Quoted2()
        {
            Assert.Equal(
                @"() => ExpressionToCode.ToCode(() => true).Length > 5",
                ExpressionToCode.ToCode(() => ExpressionToCode.ToCode(() => true).Length > 5));
        }

        [Fact]
        public void QuotedWithAnonymous()
        {
            Assert.Equal(
                @"() => new[] { new { X = ""a"", Y = ""b"" } }.Select(o => o.X + o.Y).Single()",
                ExpressionToCode.ToCode(() => new[] { new { X = "a", Y = "b" } }.Select(o => o.X + o.Y).Single()));
        }

        [Fact]
        public void StaticCall()
        {
            Assert.Equal(
                @"() => object.Equals((object)3, (object)0)",
                ExpressionToCode.ToCode(() => Equals(3, 0)));
        }

        [Fact]
        public void ThisCall()
        {
            Assert.Equal(
                @"() => !Equals((object)3)",
                ExpressionToCode.ToCode(() => !Equals(3)));
        }

        [Fact]
        public void ThisExplicit()
        {
            Assert.Equal(
                @"() => object.Equals(this, (object)3)",
                ExpressionToCode.ToCode(() => object.Equals(this, 3)));
        }

        [Fact]
        public void TypedConstant()
        {
            Assert.Equal(
                @"() => new[] { typeof(int), typeof(string) }",
                ExpressionToCode.ToCode(() => new[] { typeof(int), typeof(string) }));
        }

        [Fact]
        public void StaticCallImplicitCast()
        {
            Assert.Equal(
                @"() => object.Equals((object)3, (object)0)",
                ExpressionToCode.ToCode(() => Equals(3, 0)));
        }

        [Fact]
        public void StaticMembers()
        {
            Assert.Equal(
                @"() => (DateTime.Now > DateTime.Now + TimeSpan.FromMilliseconds(10.001)).ToString() == ""False""",
                ExpressionToCode.ToCode(
                    () => (DateTime.Now > DateTime.Now + TimeSpan.FromMilliseconds(10.001)).ToString() == "False"));
        }

        [Fact]
        public void Strings2()
        {
            var x = "X";
            const string y = "Y";
            Assert.Equal(
                @"() => x != ""Y"" && x.Length == ""Y"".Length && ""a"".Length == 1",
                ExpressionToCode.ToCode(() => x != y && x.Length == y.Length && "a".Length == 1));
        }

        [Fact]
        public void StringAccessor()
        {
            Assert.Equal(
                @"() => ""abc""[1] == 'b'",
                ExpressionToCode.ToCode(() => "abc"[1] == 'b'));
        }

        [Fact]
        public void StringConcat()
        {
            var x = "X";
            Assert.Equal(
                @"() => ((""a\n\\b"" ?? x) + x).Length == 2 ? false : true",
                ExpressionToCode.ToCode(() => (("a\n\\b" ?? x) + x).Length == 2 ? false : true));
        }

        [Fact]
        public void ArgumentWithRefModifier()
        {
            var x = "a";
            Assert.Equal(
                @"() => MethodWithRefParam(ref x)",
                ExpressionToCode.ToCode(() => MethodWithRefParam(ref x)));
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        T MethodWithRefParam<T>(ref T input) => input;

        [Fact]
        public void ArgumentWithOutModifier()
        {
            var x = "a";
            string y;
            Assert.Equal(
                @"() => MethodWithOutParam(ref x, out y)",
                ExpressionToCode.ToCode(() => MethodWithOutParam(ref x, out y)));
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        T MethodWithOutParam<T>(ref T input, out T output)
        {
            return output = input;
        }

        [Fact]
        public void StaticMethodWithRefAndOutModifiers()
        {
            var x = "a";
            object y;
            Assert.Equal(
                @"() => ClassA.MethodWithOutAndRefParam(ref x, out y, 3)",
                ExpressionToCode.ToCode(() => ClassA.MethodWithOutAndRefParam(ref x, out y, 3)));
        }

        [Fact]
        public void ConstructorMethodWithRefAndOutModifiers()
        {
            var x = 42;
            int y;
            Assert.Equal(
                @"() => new ClassA(ref x, out y)",
                ExpressionToCode.ToCode(() => new ClassA(ref x, out y)));
        }

        [Fact]
        public void ExtensionMethodWithRefAndOutModifiers()
        {
            var x = 42;
            long y;
            Assert.Equal(
                @"() => DateTime.Now.AnExtensionMethod(ref x, 5, out y)",
                ExpressionToCode.ToCode(() => DateTime.Now.AnExtensionMethod(ref x, 5, out y)));
        }

        [Fact]
        public void DelegateCallWithRefAndOutModifiers()
        {
            var x = 42;
            int y;
            DelegateWithRefAndOut myDelegate = (ref int someVar, out int anotherVar) => anotherVar = someVar;
            Assert.Equal(
                @"() => myDelegate(ref x, out y)",
                ExpressionToCode.ToCode(() => myDelegate(ref x, out y)));
        }

        [Fact]
        public void LambdaInvocation_Func_int()
        {
            Assert.Equal(
                "() => new Func<int>(() => 1)()",
                ExpressionToCode.ToCode(() => new Func<int>(() => 1)()));
        }

        [Fact]
        public void LambdaInvocation_CustomDelegate()
        {
            Assert.Equal(
                "() => new CustomDelegate(n => n + 1)(1)",
                ExpressionToCode.ToCode(() => new CustomDelegate(n => n + 1)(1)));
        }

        [Fact]
        public void FullTypeName_IfCorrespondingRuleSpecified()
        {
            Assert.Equal(
                "() => new ExpressionToCodeTest.ClassA()",
                ExpressionToCodeConfiguration.DefaultCodeGenConfiguration.WithObjectStringifier(ObjectStringify.WithFullTypeNames)
                    .GetExpressionToCode()
                    .ToCode(() => new ClassA()));
        }

        [Fact]
        public void FullTypeName_WhenRequestedEvenForMessyGenerics()
        {
            Assert.Equal(
                "() => typeof(ExpressionToCodeTest.Outer<int, int>.Nested<string>)",
                ExpressionToCodeConfiguration.DefaultCodeGenConfiguration.WithObjectStringifier(ObjectStringify.WithFullTypeNames)
                    .GetExpressionToCode()
                    .ToCode(() => typeof(Outer<int, int>.Nested<string>)));
        }

        [Fact]
        public void PlainTypeName_WhenRequestedEvenForMessyGenerics()
        {
            Assert.Equal(
                "() => typeof(Outer<int, int>.Nested<string>)",
                ExpressionToCodeConfiguration.DefaultCodeGenConfiguration.GetExpressionToCode().ToCode(() => typeof(Outer<int, int>.Nested<string>)));
        }

        [Fact]
        public void PlainTypeName_WhenFullTypeNameRequestedForUnboundTypeArgument()
        {
            var unboundListTypeArg = typeof(List<>).GetGenericArguments()[0];
            Assert.Equal(
                "T",
                ObjectStringify.WithFullTypeNames.TypeNameToCode(unboundListTypeArg));
        }

        [Fact]
        public void FullTypeName_ForNestedType()
        {
            var code = ExpressionToCodeConfiguration.DefaultCodeGenConfiguration.WithObjectStringifier(ObjectStringify.WithFullTypeNames)
                .GetExpressionToCode()
                .ToCode(() => new ExpressionToCodeTest.ExpressionToCodeLibTest.B());
            Assert.Equal("() => new ExpressionToCodeTest.ExpressionToCodeLibTest.B()", code);
        }

        class B { }

        [Fact]
        public void ThisPropertyAccess()
        {
            Assert.Equal("() => TheProperty", ExpressionToCode.ToCode(() => TheProperty));
        }

        [Fact]
        public void ThisProtectedPropertyAccess()
        {
            var code = ExpressionToCode.ToCode(() => TheProtectedProperty);
            Assert.Equal("() => TheProtectedProperty", code);
        }

        [Fact]
        public void ThisProtectedWithPrivateSetterPropertyAccess()
        {
            var code = ExpressionToCode.ToCode(() => TheProtectedWithPrivateSetterProperty);
            Assert.Equal("() => TheProtectedWithPrivateSetterProperty", code);
        }

        [Fact]
        public void ThePrivateStaticPropertyAccess()
        {
            var code = ExpressionToCode.ToCode(() => ThePrivateStaticProperty);
            Assert.Equal("() => ExpressionToCodeLibTest.ThePrivateStaticProperty", code);
        }

        [Fact]
        public void ThisMethodCall()
        {
            var code = ExpressionToCode.ToCode(() => ReturnZero());
            Assert.Equal("() => ReturnZero()", code);
        }

        [Fact]
        public void ThisStaticMethodCall()
        {
            var code = ExpressionToCode.ToCode(() => StaticReturnZero());
            Assert.Equal("() => ExpressionToCodeLibTest.StaticReturnZero()", code);
        }

        public int ReturnZero()
            => 0;

        public static int StaticReturnZero()
            => 0;

        [Fact]
        public void ThisIndexedProperty()
        {
            var actual = ExpressionToCode.ToCode(() => this[1]);
            Assert.Equal("() => this[1]", actual);
        }

        [Fact]
        public void DetectsClosuresInNestedClasses()
        {
            var expr = new ClassWithClosure {
                someValue = "test "
            }.GetExpression(DayOfWeek.Friday);
            Assert.Equal(@"() => someValue + (object)closedVariable + "" "" + (object)argument", expr);
        }

        class ClassWithClosure
        {
            public string someValue;

            public string GetExpression(DayOfWeek argument)
            {
                var arr = new[] { 37 };
                foreach (var closedVariable in arr) {
                    return ExpressionToCode.ToCode(() => someValue + closedVariable + " " + argument);
                }
                throw new Exception();
            }
        }

        public string this[int index] => "TheIndexedValue";
        public string TheProperty => "TheValue";
        protected string TheProtectedProperty => "TheValue";
        static string ThePrivateStaticProperty => "TheValue";
        protected string TheProtectedWithPrivateSetterProperty { private get; set; }
    }

    public delegate int DelegateWithRefAndOut(ref int someVar, out int anotherVar);

    public delegate int CustomDelegate(int input);

    static class StaticHelperClass
    {
        public static long AnExtensionMethod(this DateTime date, ref int tickOffset, int dayOffset, out long alternateOut)
        {
            return alternateOut = date.AddDays(dayOffset).Ticks + tickOffset;
        }
    }

    class ClassA
    {
        public static int MethodWithOutAndRefParam<T>(ref T input, out object output, int x)
        {
            output = x == 4 ? default(object) : input;
            return x;
        }

        int x;
        public ClassA() { }

        public ClassA(ref int something, out int output) => output = x = something;

        public void DoAssert()
        {
            x = 37;
            Assert.Equal(
                @"() => x != C()",
                ExpressionToCode.ToCode(() => x != C()));
            Assert.Equal(
                @"() => !object.ReferenceEquals(this, new ClassA())",
                ExpressionToCode.ToCode(() => !ReferenceEquals(this, new ClassA())));
            Assert.Equal(
                @"() => MyEquals(this) && !MyEquals(default(ClassA))",
                ExpressionToCode.ToCode(() => MyEquals(this) && !MyEquals(default(ClassA))));
        }

        int C() => x + 5;
        bool MyEquals(ClassA other) => other != null && x == other.x;
    }
}
