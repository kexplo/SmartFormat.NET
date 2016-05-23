﻿using System.Collections.Generic;
#if !UNITY_5
using System.Dynamic;
#endif
using NUnit.Framework;
using SmartFormat.Core.Settings;
using SmartFormat.Extensions;
using SmartFormat.Tests.TestUtils;

namespace SmartFormat.Tests.Extensions
{
	[TestFixture]
	public class DictionaryFormatterTests
	{
		public object[] GetArgs()
		{
			var d = new Dictionary<string, object>() {
				{"Numbers", new Dictionary<string, object>() {
					{"One", 1},
					{"Two", 2},
					{"Three", 3},
				}},
				{"Letters", new Dictionary<string, object>() {
					{"A", "a"},
					{"B", "b"},
					{"C", "c"},
				}},
				{"Object", new {
					Prop1 = "a",
					Prop2 = "b",
					Prop3 = "c",
				}},
			};

			return new object[] {
				d,
			};
		}

#if !UNITY_5
		public dynamic GetDynamicArgs()
		{
			dynamic d = new ExpandoObject();
			d.Numbers = new Dictionary<string, object> { { "One", 1 }, { "Two", 2 }, { "Three", 3 }, };
			d.Letters = new Dictionary<string, object> { { "A", "a" }, { "B", "b" }, { "C", "c" }, };
			d.Raw = new System.Collections.Hashtable { { "X", "z" } };
			d.Object = new { Prop1 = "a", Prop2 = "b", Prop3 = "c", };

			return new object[] {
				d,
			};
		}
#endif

		[Test]
		public void Test_Dictionary()
		{
			var formatter = Smart.CreateDefaultSmartFormat();
			formatter.AddExtensions(new DictionarySource(formatter));

			var formats = new string[] {
				"Chained: {0.Numbers.One} {Numbers.Two} {Letters.A} {Object.Prop1} ",
				"Nested: {0:{Numbers:{One} {Two}} } {Letters:{A}} {Object:{Prop1}} ", // Due to double-brace escaping, the spacing in this nested format is irregular
			};
			var expected = new string[] {
				"Chained: 1 2 a a ",
				"Nested: 1 2  a a ",
			};
			var args = GetArgs();
			formatter.Test(formats, args, expected);
		}

#if !UNITY_5
		[Test]
		public void Test_Dynamic()
		{
			var formatter = Smart.CreateDefaultSmartFormat();
			formatter.AddExtensions(new DictionarySource(formatter));

			var formats = new string[] {
				"Chained: {0.Numbers.One} {Numbers.Two} {Letters.A} {Object.Prop1} {Raw.X} ",
				"Nested: {0:{Numbers:{One} {Two}} } {Letters:{A}} {Object:{Prop1}} {Raw:{X}} ", // Due to double-brace escaping, the spacing in this nested format is irregular
			};
			var expected = new string[] {
				"Chained: 1 2 a a z ",
				"Nested: 1 2  a a z ",
			};
			var args = (object[])GetDynamicArgs();
			formatter.Test(formats, args, expected);
		}

		[Test]
		public void Test_Dynamic_CaseInsensitive()
		{
			var formatter = Smart.CreateDefaultSmartFormat();
			formatter.Settings.CaseSensitivity = CaseSensitivityType.CaseInsensitive;
			formatter.AddExtensions(new DictionarySource(formatter));

			var formats = new string[] {
				"Chained: {0.Numbers.One} {Numbers.Two} {Letters.A} {Object.Prop1} {Raw.x} ",
				"Nested: {0:{Numbers:{One} {Two}} } {Letters:{A}} {Object:{Prop1}} {Raw:{x}} ", // Due to double-brace escaping, the spacing in this nested format is irregular
			};
			var expected = new string[] {
				"Chained: 1 2 a a z ",
				"Nested: 1 2  a a z ",
			};
			var args = (object[])GetDynamicArgs();
			formatter.Test(formats, args, expected);
		}
#endif
	}
}
