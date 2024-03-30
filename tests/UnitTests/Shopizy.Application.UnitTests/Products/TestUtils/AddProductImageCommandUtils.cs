using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Products.TestUtils;

public static class AddProductImageCommandUtils
{
    public static AddProductImageCommand CreateCommand(Guid productId)
    {
        var fileMock = new Mock<IFormFile>();

        var content = "Hello World from a Fake File";
        var fileName = "test.pdf";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        ms.Dispose();

        return new AddProductImageCommand(Constants.User.Id.Value, productId, fileMock.Object);
    }
}
