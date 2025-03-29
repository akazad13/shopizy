using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Products.Commands.AddProductImage;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class AddProductImageCommandUtils
{
    public static AddProductImageCommand CreateCommand(Guid userId, Guid productId)
    {
        var fileMock = new Mock<IFormFile>();

        string content = "Hello World from a Fake File";
        string fileName = "test.pdf";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);

        ms.Dispose();

        return new AddProductImageCommand(userId, productId, fileMock.Object);
    }
}
