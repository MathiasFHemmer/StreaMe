namespace Lib.Tests;
using Streame.Lib.Result;
public class ResulTests
{
    [Fact]
    public void SuccessResult_MustHaveTrueSuccess()
    {
        // Arrange
        var successValue = 1;
        var result = Result<int, Error>.Success(successValue);

        // Act
        var isSuccess = result.isSuccess;

        // Assert
        Assert.True(isSuccess);
    }

    [Fact]
    public void SuccessResult_MustHaveCorrectValue()
    {
        // Arrange
        var successValue = 1;
        var result = Result<int, Error>.Success(successValue);

        // Act
        var value = result.Value;

        // Assert
        Assert.Equal(successValue, value);
    }

    [Fact]
    public void FailureResult_MustHaveError()
    {
        // Arrange
        var error = new Error("Error", "An error occurred");
        var result = Result.Failure(error);

        // Act
        var failureError = result.Error;

        // Assert
        Assert.Equal(error, failureError);
    }
    [Fact]
    public void FailedResult_MustHaveFalseSuccess()
    {
        // Arrange
        var error = new Error("Error", "An error occurred");
        var result = Result<Error>.Failure(error);

        // Act
        var isSuccess = result.IsSuccess;

        // Assert
        Assert.False(isSuccess);
    }
}
