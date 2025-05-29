using eCommerceOnlineShop.Cart.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace eCommerceOnlineShop.Cart.Tests.Handlers
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
        private readonly GlobalExceptionHandler _handler;

        public GlobalExceptionHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
            _handler = new GlobalExceptionHandler(_loggerMock.Object);
        }

        [Fact]
        public async Task TryHandleAsync_ArgumentNullException_ReturnsBadRequest()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new ArgumentNullException("param", "Parameter cannot be null");
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            // Act
            var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);

            responseStream.Position = 0;
            var response = await JsonSerializer.DeserializeAsync<ErrorResponse>(responseStream);
            Assert.NotNull(response);
            Assert.Equal("Parameter cannot be null", response.Error.Message);
            Assert.Equal("ArgumentNullException", response.Error.Type);
        }

        [Fact]
        public async Task TryHandleAsync_UnknownException_ReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Something went wrong");
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            // Act
            var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);

            responseStream.Position = 0;
            var response = await JsonSerializer.DeserializeAsync<ErrorResponse>(responseStream);
            Assert.NotNull(response);
            Assert.Equal("Something went wrong", response.Error.Message);
            Assert.Equal("Exception", response.Error.Type);
        }

        private class ErrorResponse
        {
            public ErrorDetails Error { get; set; } = new();
        }

        private class ErrorDetails
        {
            public string Message { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }
    }
}
