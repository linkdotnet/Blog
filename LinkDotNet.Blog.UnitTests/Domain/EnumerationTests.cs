using System;
using FluentAssertions;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class EnumerationTests
{
    [Fact]
    public void GivenEnumeration_WhenCallingToString_ThenKeyIsReturned()
    {
        // Arrange
        var testEnum = TestEnumeration.One;

        // Act
        var display = testEnum.ToString();

        // Assert
        display.Should().Be(testEnum.Key);
    }

    [Fact]
    public void GivenEnumerationWhenCallingAll_ThenAllPartsAreReturned()
    {
        // Act
        var all = TestEnumeration.All;

        // Assert
        all.Should().Contain(TestEnumeration.One);
        all.Should().Contain(TestEnumeration.Two);
    }

    [Fact]
    public void GivenEnumerationObject_WhenComparingEqualOnes_ThenEqual()
    {
        // Arrange
        var enum1 = TestEnumeration.One;
        var alsoEnum1 = TestEnumeration.Create("One");

        // Act
        var isEqual = enum1 == alsoEnum1;

        isEqual.Should().BeTrue();
    }

    [Fact]
    public void GivenInvalidKey_WhenCreatingEnumeration_ThenErrorResult()
    {
        // Act
        Action result = () => TestEnumeration.Create("InvalidKey");

        // Assert
        result.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void GivenNullOrEmptyKey_WhenCreating_ThenException(string key)
    {
        Action result = () => new TestEnumeration(key);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void NullEnumerationNeverEqual()
    {
        var isEqual = null == (TestEnumeration)null;

        isEqual.Should().BeFalse();
    }

    [Fact]
    public void ShouldDifferentEnumerations()
    {
        var isEqual = TestEnumeration.One != TestEnumeration.Two;

        isEqual.Should().BeTrue();
    }

    [Fact]
    public void GivenTwoEqualEnumerationThenHashcodeEqual()
    {
        var hashCode1 = TestEnumeration.One.GetHashCode();
        var hashCode2 = TestEnumeration.Create(TestEnumeration.One.Key).GetHashCode();

        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void ShouldNotBeEqualWhenNull()
    {
        var isEqual = TestEnumeration.One.Equals(null);

        isEqual.Should().BeFalse();
    }

    [Fact]
    public void ShouldNotBeEqualWhenDifferentNull()
    {
        var isEqual = TestEnumeration.One.Equals("string");

        isEqual.Should().BeFalse();
    }

    [Fact]
    public void ShouldBeEqualWhenKeyTheSame()
    {
        var isEqual = TestEnumeration.One.Equals(TestEnumeration.One);

        isEqual.Should().BeTrue();
    }
}
