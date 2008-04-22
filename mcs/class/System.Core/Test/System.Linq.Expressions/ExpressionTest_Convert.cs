//
// ExpressionTest_Convert.cs
//
// Author:
//   Jb Evain (jbevain@novell.com)
//
// (C) 2008 Novell, Inc. (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace MonoTests.System.Linq.Expressions {

	[TestFixture]
	public class ExpressionTest_Convert {

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void NullExpression ()
		{
			Expression.Convert (null, typeof (int));
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))]
		public void NullType ()
		{
			Expression.Convert (1.ToConstant (), null);
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ConvertIntToString ()
		{
			Expression.Convert (1.ToConstant (), typeof (string));
		}

		interface IFoo { }
		class Foo : IFoo { }
		class Bar : Foo { }
		class Baz { }

		interface ITzap { }

		[Test]
		public void ConvertBackwardAssignability ()
		{
			var c = Expression.Convert (
				Expression.Constant (null, typeof (Bar)), typeof (Foo));
		}

		[Test]
		public void ConvertInterfaces ()
		{
			var p = Expression.Parameter (typeof (IFoo), null);

			var conv = Expression.Convert (p, typeof (ITzap));
			Assert.AreEqual (typeof (ITzap), conv.Type);

			p = Expression.Parameter (typeof (ITzap), null);
			conv = Expression.Convert (p, typeof (IFoo));

			Assert.AreEqual (typeof (IFoo), conv.Type);
		}

		[Test]
		public void ConvertCheckedInt32ToInt64 ()
		{
			var c = Expression.ConvertChecked (
				Expression.Constant (2, typeof (int)), typeof (long));

			Assert.AreEqual (ExpressionType.ConvertChecked, c.NodeType);
		}

		[Test]
		public void ConvertCheckedFallbackToConvertForNonPrimitives ()
		{
			var p = Expression.ConvertChecked (
				Expression.Constant (null, typeof (object)), typeof (IFoo));

			Assert.AreEqual (ExpressionType.Convert, p.NodeType);
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ConvertBazToFoo ()
		{
			Expression.Convert (Expression.Parameter (typeof (Baz), ""), typeof (Foo));
		}

		struct EineStrukt { }

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ConvertStructToFoo ()
		{
			Expression.Convert (Expression.Parameter (typeof (EineStrukt), ""), typeof (Foo));
		}

		[Test]
		[ExpectedException (typeof (InvalidOperationException))]
		public void ConvertInt32ToBool ()
		{
			Expression.Convert (Expression.Parameter (typeof (int), ""), typeof (bool));
		}

		[Test]
		public void ConvertIFooToFoo ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (IFoo), ""), typeof (Foo));
			Assert.AreEqual (typeof (Foo), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		[Test]
		public void BoxInt32 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (int), ""), typeof (object));
			Assert.AreEqual (typeof (object), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		[Test]
		public void UnBoxInt32 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (object), ""), typeof (int));
			Assert.AreEqual (typeof (int), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		[Test]
		public void ConvertInt32ToInt64 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (int), ""), typeof (long));
			Assert.AreEqual (typeof (long), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		[Test]
		public void ConvertInt64ToInt32 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (long), ""), typeof (int));
			Assert.AreEqual (typeof (int), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		enum EineEnum { }

		[Test]
		public void ConvertEnumToInt32 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (EineEnum), ""), typeof (int));
			Assert.AreEqual (typeof (int), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		[Test]
		[Category ("NotWorking")]
		public void ConvertNullableInt32ToInt32 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (int?), ""), typeof (int));
			Assert.AreEqual (typeof (int), c.Type);
			Assert.IsTrue (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}

		[Test]
		[Category ("NotWorking")]
		public void ConvertInt32ToNullableInt32 ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (int), ""), typeof (int?));
			Assert.AreEqual (typeof (int?), c.Type);
			Assert.IsTrue (c.IsLifted);
			Assert.IsTrue (c.IsLiftedToNull);
			Assert.IsNull (c.Method);
		}


		class Klang {
			int i;

			public Klang (int i)
			{
				this.i = i;
			}

			public static explicit operator int (Klang k)
			{
				return k.i;
			}
		}

		[Test]
		public void ConvertClassWithExplicitOp ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (Klang), ""), typeof (int));
			Assert.AreEqual (typeof (int), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNotNull (c.Method);
		}

		[Test]
		[Category ("NotWorking")]
		public void ConvertClassWithExplicitOpToNullableInt ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (Klang), ""), typeof (int?));
			Assert.AreEqual (typeof (int?), c.Type);
			Assert.IsTrue (c.IsLifted);
			Assert.IsTrue (c.IsLiftedToNull);
			Assert.IsNotNull (c.Method);
		}

		class Kling {
			int i;

			public Kling (int i)
			{
				this.i = i;
			}

			public static implicit operator int (Kling k)
			{
				return k.i;
			}
		}

		[Test]
		public void ConvertClassWithImplicitOp ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (Kling), ""), typeof (int));
			Assert.AreEqual (typeof (int), c.Type);
			Assert.IsFalse (c.IsLifted);
			Assert.IsFalse (c.IsLiftedToNull);
			Assert.IsNotNull (c.Method);
		}

		[Test]
		[Category ("NotWorking")]
		public void ConvertClassWithImplicitOpToNullableInt ()
		{
			var c = Expression.Convert (Expression.Parameter (typeof (Kling), ""), typeof (int?));
			Assert.AreEqual (typeof (int?), c.Type);
			Assert.IsTrue (c.IsLifted);
			Assert.IsTrue (c.IsLiftedToNull);
			Assert.IsNotNull (c.Method);
		}
	}
}
