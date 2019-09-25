using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Generator.Shared.Transformation;
using Xunit;

namespace Generator.Tests
{
	public class AssemblyNameAlignerTests
	{
		[Fact]
		public void VerifyThrowOptions()
		{
			Assert.Throws(typeof(ArgumentNullException), () => AssemblyNameAligner.Execute("test", Enumerable.Empty<string>().ToList()));
		}

		[Fact]
		public void VerifyThrowRequest()
		{
			Assert.Throws(typeof(ArgumentNullException), () => AssemblyNameAligner.Execute(string.Empty, new List<string>(){"a.b", "a.b.c"}));
		}

		[Theory]
		[InlineData(new string[]{"a.b.c"}, new string []{""})]
		public void VerifySingleInput(string[] input, string[] output)
		{
			if(input.Length != output.Length)
				throw new Exception($"Invalid test configuration - Uneven amount of input/output");

			for (var index = 0; index < input.Length; index++)
			{
				AssemblyNameAligner.Execute(input[index], input).Should().Be(output[index]);
			}
		}

		[Theory]
		[InlineData(new[] {"a.b.c", "a.b.c.d", "a.b.c.e" }, 
			new[]{ "", "d", "e" })]
		[InlineData(new[] {"a.b", "a.b.c", "a.b.d" }, 
			new[]{ "", "c", "d" })]
		[InlineData(new[] {"d.b.c", "a.b.c.d", "a.b.c.e" }, 
			new[]{ "d.b.c", "a.b.c.d", "a.b.c.e" })]
		[InlineData(new[] {"Model.Entities", "Model.EntityFramework"}, 
			new[]{ "Entities", "EntityFramework" })]
		public void VerifyMultiInput(string[] input, string[] output)
		{
			if (input.Length != output.Length)
				throw new Exception($"Invalid test configuration - Uneven amount of input/output");

			for (var index = 0; index < input.Length; index++)
			{
				AssemblyNameAligner.Execute(input[index], input).Should().Be(output[index]);
			}
		}
	}
}